using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using SlimDX.Direct3D10;
using System.Windows.Media;
using DirectCanvas.Misc;
using DirectCanvas.Scenes;

namespace DirectCanvas
{
	/// <summary>
	/// WPFPresenter renders DirectCanvas content to WPF airspace.
	/// </summary>
	public sealed class WPFPresenter : Presenter
    {
        /// <summary>
        /// Cached pixel width size of presenter layer
        /// </summary>
        private int m_width;

        /// <summary>
        /// Cached pixel hight size of presenter layer
        /// </summary>
        private int m_height;

        RectangleF rect;
        Int32Rect intRect;
        Color4 white = new Color4(1, 1, 1, 1);

        /// <summary>
        /// The WPF D3DImage class that is set to the WPF Image source
        /// to render our Direct3D content
        /// </summary>
        private D3DImageSlimDX m_d3dImage;
        public ImageSource ImageSource
        {
            get
            {
                return m_d3dImage;
            }
        }

        public bool IsRenderingActive { get; private set; }

        private Scene _currentScene;
        public Scene CurrentScene
        {
            get
            {
                return _currentScene;
            }
            set
            {
                if (_currentScene == value)
                    return;

                if (_currentScene != null)
                {
                    _currentScene.DeactivateScene();
                }

                _currentScene = value;

                if (_currentScene != null)
                {
                    _currentScene.ActivateScene();
                }
            }
        }

        /// <summary>
        /// The internal layer which everything is drawn to
        /// </summary>
        private DrawingLayer m_layer1;

        private DrawingLayer m_layer2;

        public WPFPresenter(DirectCanvasFactory directCanvas, int width, int height)
            : base(directCanvas)
        {
            // Check and see if we are on the UI thread as we may want to support multi-threaded scenarios.
            InitD3DImage(width, height);
        }

        /// <summary>
        /// Creates a new WPFPresenter instance.
        /// </summary>
        /// <param name="directCanvas">The DirectCanvas factor this presenter belongs to</param>
        /// <param name="width">The pixel width of the presenter</param>
        /// <param name="height">The pixel height of the presenter</param>
        /// <param name="image">The WPF Image to render to</param>
        public WPFPresenter(DirectCanvasFactory directCanvas, int width, int height, Image image)
            : this(directCanvas, width, height)
        {
            // Check and see if we are on the UI thread as we may want to support multi-threaded scenarios.
            if (!image.Dispatcher.CheckAccess())
            {
                image.Dispatcher.Invoke((Action)delegate
                {
                    image.Source = m_d3dImage;
                });
            }
            else
            {
                image.Source = m_d3dImage;
            }
        }

        protected override DrawingLayer Layer
        {
            get
            {
                return m_layer1;
            }
        }

        /// <summary>
        /// Sets the size of the presenter
        /// </summary>
        /// <param name="width">Pixel width of the presenter</param>
        /// <param name="height">Pixel height of the presenter</param>
        public override void SetSize(int width, int height)
        {
            InitD3DImage(width, height);

            base.SetSize(width, height);
        }

        private void InitD3DImage(int width, int height)
        {
            if (Application.Current == null ||
                Application.Current.Dispatcher == null)
                return;
            var dispatcher = Application.Current.Dispatcher;

            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke((Action)delegate
                {
                    InitD3DImage(width, height);
                });
                return;
            }

            ReleaseResources();

            m_width = width;
            m_height = height;

            rect = new RectangleF(0, 0, m_width, m_height);
            intRect = new Int32Rect(0, 0, m_width, m_height);

            if (m_d3dImage == null)
            {
                m_d3dImage = new D3DImageSlimDX();
                m_d3dImage.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;
            }

            m_layer1 = new DrawingLayer(Factory,
                                       width,
                                       height,
                                       SlimDX.DXGI.Format.B8G8R8A8_UNorm,
                                       ResourceOptionFlags.Shared);

            m_layer2 = new DrawingLayer(Factory,
                                           width,
                                           height,
                                           SlimDX.DXGI.Format.B8G8R8A8_UNorm,
                                           ResourceOptionFlags.Shared);

            SetBackBuffer();
        }

        private void SetBackBuffer()
        {
            m_d3dImage.SetBackBufferSlimDX(m_layer2.RenderTargetTexture.InternalTexture2D);
        }

        /// <summary>
        /// Presents the DirectCanvas content to WPF
        /// </summary>
        public override void Present()
        {
            DoRender();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            DoRender();
        }

        private void DoRender()
        {
            if (m_d3dImage != null &&
                !m_d3dImage.CheckAccess())
            {
                m_d3dImage.Dispatcher.Invoke((Action)delegate
                {
                    DoRender();
                });
                return;
            }
            if (!m_d3dImage.IsFrontBufferAvailable)
                return;

            Scene scene = CurrentScene;
            if (scene != null)
            {
                scene.Render();
            }

            m_d3dImage.Lock();

            m_layer2.BeginCompose();
            m_layer2.Clear();
            
            m_layer2.ComposeLayer(Layer, rect, rect, Misc.RotationParameters.Empty, white);
            m_layer2.EndCompose();

            Factory.DeviceContext.Device.Flush();

            m_d3dImage.AddDirtyRect(intRect);
            m_d3dImage.Unlock();
        }

        public void StartRendering()
        {
            if (m_d3dImage.IsFrontBufferAvailable)
            {
                SetBackBuffer();
                CompositionTarget.Rendering += OnRendering;
                IsRenderingActive = true;
            }
        }

        public void StopRendering()
        {
            CompositionTarget.Rendering -= OnRendering;
            IsRenderingActive = false;
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // This fires when the screensaver kicks in, the machine goes into sleep or hibernate
            // and any other catastrophic losses of the d3d device from WPF's point of view
            if (m_d3dImage.IsFrontBufferAvailable)
            {
                StartRendering();
            }
            else
            {
                StopRendering();
            }
        }

        private void ReleaseResources()
        {
            if (m_layer1 != null)
            {
                m_layer1.Dispose();
                m_layer1 = null;
            }

            if (m_layer2 != null)
            {
                m_layer2.Dispose();
                m_layer2 = null;
            }
        }

        public override void Dispose()
        {
            if (m_d3dImage != null)
                m_d3dImage.Dispatcher.Invoke((Action)delegate
                {
                    m_d3dImage.Lock();
                    m_d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                    m_d3dImage.Unlock();
                    m_d3dImage.Dispose();
                    m_d3dImage = null;
                    ReleaseResources();
                });


            base.Dispose();
        }
    }
}