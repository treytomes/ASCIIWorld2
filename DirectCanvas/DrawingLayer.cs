using System;
using System.Collections.Generic;
using DirectCanvas.Effects;
using DirectCanvas.Helpers;
using DirectCanvas.Imaging;
using DirectCanvas.Misc;
using DirectCanvas.Multimedia;
using DirectCanvas.Rendering.Materials;
using DirectCanvas.Shapes;
using DirectCanvas.Transforms;
using SlimDX;
using SlimDX.Direct2D;
using SlimDX.Direct3D10;
using SlimDX.DirectWrite;
using SlimDX.DXGI;
using Brush = DirectCanvas.Brushes.Brush;
using Factory = SlimDX.DirectWrite.Factory;
using FactoryType = SlimDX.DirectWrite.FactoryType;
using FontWeight = SlimDX.DirectWrite.FontWeight;
using StateBlock = SlimDX.Direct2D.StateBlock;

namespace DirectCanvas
{
    /// <summary>
    /// The DrawingLayer is a surface in which drawing and rendering can take place.
    /// </summary>
    public class DrawingLayer : IDisposable
    {
        /// <summary>
        /// Reference to the DirectCanvasFactory
        /// </summary>
        private readonly DirectCanvasFactory m_directCanvasFactory;

        /// <summary>
        /// The color format of the surface
        /// </summary>
        private readonly Format m_format;

        /// <summary>
        /// The Direct3D backed render target
        /// </summary>
        private RenderTargetTexture m_renderTargetTexture;
        
        /// <summary>
        /// The Direct2D wrapped render target for the Direct3D surface (m_renderTargetTexture)
        /// </summary>
        private Direct2DRenderTarget m_d2DRenderTarget;

        /// <summary>
        /// Stack to push/pop the Direct2D render target state
        /// </summary>
        private Stack<StateBlock> m_stateStack = new Stack<StateBlock>();

        private DrawStateManagement m_drawStateManagement;

        private bool m_enableImageCopy;

        /// <summary>
        /// Pushes the current DrawingLayer state
        /// </summary>
        internal void PushState()
        {
            var state = new StateBlock(D2DRenderTarget.InternalRenderTarget.Factory);
            D2DRenderTarget.InternalRenderTarget.SaveDrawingState(state);
            m_stateStack.Push(state);
        }

        /// <summary>
        /// Pops back the current DrawingLayer state
        /// </summary>
        internal void PopState()
        {
            var state = m_stateStack.Pop();
            D2DRenderTarget.InternalRenderTarget.RestoreDrawingState(state);
            state.Dispose();
        }

        /// <summary>
        /// Pointer to the Direct3D Texture2D. Use this to render to the DrawingLayer
        /// using other Direct3D libraries.
        /// </summary>
        public IntPtr Texture2DComPointer
        {
            get
            {
                if (m_renderTargetTexture == null ||
                    m_renderTargetTexture.InternalTexture2D == null)
                    return IntPtr.Zero;
                return m_renderTargetTexture.InternalTexture2D.ComPointer;
            }
        }

        /// <summary>
        /// Pointer to the Direct3D10 RenderTargetView. Use this to render to the DrawingLayer
        /// using other Direct3D libraries.
        /// </summary>
        public IntPtr RenderTargetViewComPointer
        {
            get
            {
                if (m_renderTargetTexture == null ||
                    m_renderTargetTexture.InternalRenderTargetView == null)
                    return IntPtr.Zero;
                return m_renderTargetTexture.InternalRenderTargetView.ComPointer;
            }
        }

        public bool EnableImageCopy
        {
            get { return m_enableImageCopy; }
            set
            {
                if(m_enableImageCopy != value)
                {
                    m_enableImageCopy = value;
                    OnEnableImageCopyChanged();
                }
            }
        }

        private void OnEnableImageCopyChanged()
        {
            if(!EnableImageCopy)
            {
                if(SystemMemoryTexture != null)
                {
                    SystemMemoryTexture.Dispose();
                    SystemMemoryTexture = null;
                }

                return;
            }

            if (SystemMemoryTexture != null)
                return;

            EnsureSystemMemoryTexture();
        }

        protected void EnsureSystemMemoryTexture()
        {
            if (SystemMemoryTexture != null)
            {
                SystemMemoryTexture.Dispose();
                SystemMemoryTexture = null;
            }

            if (EnableImageCopy == false)
                return;

            var device = m_directCanvasFactory.DeviceContext.Device;

            var description = RenderTargetTexture.Description;

            SystemMemoryTexture = new StagingTexture(device,
                                                    description.Width,
                                                    description.Height,
                                                    description.Format);
        }

        internal StagingTexture SystemMemoryTexture { get; private set; }

        /// <summary>
        /// The width, in pixels, of the DrawingLayer
        /// </summary>
        public int Width
        {
            get { return RenderTargetTexture.Description.Width; }
        }

        /// <summary>
        /// The height, in pixels, of the DrawingLayer
        /// </summary>
        public int Height
        {
            get { return RenderTargetTexture.Description.Height; }
        }

        /// <summary>
        /// Creates a new instance of the DrawingLayer
        /// </summary>
        /// <param name="directCanvasFactory">The factory that the DrawingLayer belongs to</param>
        /// <param name="width">The pixel size in width to make the D3D texture</param>
        /// <param name="height">The pixel size in height to make the D3D texture</param>
        /// <param name="format">The color format to make the D3D texture</param>
        /// <param name="optionFlags">Option flags to use on the creation of the D3D texture</param>
        internal DrawingLayer(DirectCanvasFactory directCanvasFactory, 
                              int width, 
                              int height, 
                              Format format = Format.B8G8R8A8_UNorm, 
                              ResourceOptionFlags optionFlags = ResourceOptionFlags.None)
        {
            m_directCanvasFactory = directCanvasFactory;
            m_format = format;

            m_drawStateManagement = new DrawStateManagement();

            var device = m_directCanvasFactory.DeviceContext.Device;

            /* Create our Direct3D texture */
            RenderTargetTexture = new RenderTargetTexture(device, width, height, m_format, optionFlags);

            /* Create the Direct2D render target, using our Direct3D texture we just created */
            D2DRenderTarget = new Direct2DRenderTarget(m_directCanvasFactory.DeviceContext,
                                                       m_renderTargetTexture.InternalDxgiSurface, 
                                                       m_format);
        }

        /// <summary>
        /// Creates a new instance of the DrawingLayer
        /// </summary>
        /// <param name="directCanvasFactory">The factory that the DrawingLayer belongs to</param>
        /// <param name="renderTargetTexture">The Direct3D texture to use</param>
        internal DrawingLayer(DirectCanvasFactory directCanvasFactory, RenderTargetTexture renderTargetTexture)
        {
            m_directCanvasFactory = directCanvasFactory;
            m_format = renderTargetTexture.Description.Format;

            m_drawStateManagement = new DrawStateManagement();

            /* Set our Direct3D texture */
            RenderTargetTexture = renderTargetTexture;

            /* Create the Direct2D render target, using our Direct3D texture we were passed */
            D2DRenderTarget = new Direct2DRenderTarget(m_directCanvasFactory.DeviceContext,
                                                       m_renderTargetTexture.InternalDxgiSurface, 
                                                       m_format);
        }

        /// <summary>
        /// Creates a new instance of the DrawingLayer.  This is the constructor that 
        /// presenter subclasses will use, making sure they override the D2DRenderTarget and RenderTargetTexture
        /// </summary>
        /// <param name="directCanvasFactory">The factory that the DrawingLayer belongs to</param>
        internal DrawingLayer(DirectCanvasFactory directCanvasFactory)
        {
            m_drawStateManagement = new DrawStateManagement();
            m_directCanvasFactory = directCanvasFactory;
        }

        /// <summary>
        /// The Direct2D render target wrapper
        /// </summary>
        virtual internal Direct2DRenderTarget D2DRenderTarget
        {
            get { return m_d2DRenderTarget; }
            private set { m_d2DRenderTarget = value; }
        }

        /// <summary>
        /// The Direct3D texture wrapper
        /// </summary>
        virtual internal RenderTargetTexture RenderTargetTexture
        {
            get { return m_renderTargetTexture; }
            private set { m_renderTargetTexture = value; }
        }

        public DirectCanvasFactory Factory
        {
            get { return m_directCanvasFactory; }
        }

        /// <summary>
        /// Begins the composition process.  BeingCompose always needs 
        /// to be eventually followed by an EndCompose.
        /// </summary>
        public void BeginCompose()
        {
            m_directCanvasFactory.BeginCompose(RenderTargetTexture, BlendStateMode.AlphaBlend);
        }

        /// <summary>
        /// Begins the composition process.  BeingCompose always needs 
        /// to be eventually followed by an EndCompose.
        /// </summary>
        /// <param name="blendStateMode">The blend state to use</param>
        public void BeginCompose(BlendStateMode blendStateMode)
        {
            m_directCanvasFactory.BeginCompose(RenderTargetTexture, blendStateMode);
        }

        /// <summary>
        /// Composes a DrawingLayer with another DrawingLayer.  
        /// This allows for things like scaling, blending, 2D and 3D transformations
        /// </summary>
        /// <param name="layer">The DrawingLayer that is used as the input</param>
        /// <param name="sourceArea">The area over the input DrawingLayer</param>
        /// <param name="destinationArea">The output area to draw the source area</param>
        /// <param name="rotationTransform">The rotation parameters</param>
        /// <param name="tint">The color to tint the source layer on to the output</param>
        public void ComposeLayer(DrawingLayer layer, ref RectangleF sourceArea, ref RectangleF destinationArea, ref RotationParameters rotationTransform, ref Color4 tint)
        {
            m_directCanvasFactory.Compose(layer, ref sourceArea, ref destinationArea, ref rotationTransform, ref tint);
        }

        /// <summary>
        /// Composes a DrawingLayer with another DrawingLayer.  
        /// This allows for things like scaling, blending, 2D and 3D transformations
        /// </summary>
        /// <param name="layer">The DrawingLayer that is used as the input</param>
        /// <param name="sourceArea">The area over the input DrawingLayer</param>
        /// <param name="destinationArea">The output area to draw the source area</param>
        /// <param name="rotationTransform">The rotation parameters</param>
        /// <param name="tint">The color to tint the source layer on to the output</param>
        public void ComposeLayer(DrawingLayer layer, RectangleF sourceArea, RectangleF destinationArea, RotationParameters rotationTransform, Color4 tint)
        {
            m_directCanvasFactory.Compose(layer, sourceArea, destinationArea, rotationTransform, tint);
        }

        /// <summary>
        /// Ends the composition process and flushes any remaining commands.  
        /// A BeginCompose must have been call first.
        /// </summary>
        public void EndCompose()
        {
            m_directCanvasFactory.EndCompose();    
        }
        
        /// <summary>
        /// Begins the drawing process.  Eventually the EndDraw should be called
        /// </summary>
        public void BeginDraw()
        {
            m_drawStateManagement.BeginDrawState();
            D2DRenderTarget.InternalRenderTarget.BeginDraw();
        }

        /// <summary>
        /// Ends the drawing process and flushes all queued drawing commands.  This should happen after a BeginDraw call.
        /// </summary>
        public void EndDraw()
        {
            m_drawStateManagement.EndDrawState();
            D2DRenderTarget.InternalRenderTarget.Flush();
            D2DRenderTarget.InternalRenderTarget.EndDraw();
        }

        /// <summary>
        /// Clears the DrawingLayer with the specified color
        /// </summary>
        /// <param name="color">The color to clear the drawing layer</param>
        public void Clear(Color4 color)
        {
            /* More optimized to clear using D2D interface _IF_ BeginDraw has been called */
            if(m_drawStateManagement.BeganDraw)
                D2DRenderTarget.InternalRenderTarget.Clear(color.InternalColor4);
            else
                /* Use D3D interface to clear */
                RenderTargetTexture.Clear(color);
        }

        /// <summary>
        /// Clears the DrawingLayer
        /// </summary>
        public void Clear()
        {
            Clear(new Color4(0,0,0,0));
        }

        /// <summary>
        /// Draws a MediaPlayer's video on to the DrawingLayer
        /// </summary>
        /// <param name="player">The MediaPlayer to draw</param>
        /// <param name="destinationRectangle">The destination rectangle</param>
        /// <param name="sourceRectangle">The source rectangle</param>
        /// <param name="opacity">The transparency level, 0 - 1</param>
        public void DrawMediaPlayer(MediaPlayer player, RectangleF destinationRectangle, RectangleF sourceRectangle, float opacity)
        {
            m_drawStateManagement.DrawPreamble();

            if (player.InternalBitmap == null)
                return;

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(player.InternalBitmap, 
                                                            destinationRectangle.InternalRectangleF, 
                                                            opacity, 
                                                            InterpolationMode.Linear, 
                                                            sourceRectangle.InternalRectangleF);
        }

        /// <summary>
        /// Draws a MediaPlayer's video on to the DrawingLayer
        /// </summary>
        /// <param name="player">The MediaPlayer to draw</param>
        /// <param name="destinationRectangle">The destination rectangle</param>
        /// <param name="opacity">The transparency level, 0 - 1</param>
        public void DrawMediaPlayer(MediaPlayer player, RectangleF destinationRectangle, float opacity)
        {
            m_drawStateManagement.DrawPreamble();

            if (player.InternalBitmap == null)
                return;

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(player.InternalBitmap,
                                                            destinationRectangle.InternalRectangleF,
                                                            opacity,
                                                            InterpolationMode.Linear);
        }

        /// <summary>
        /// Draws a MediaPlayer's video on to the DrawingLayer
        /// </summary>
        /// <param name="player">The MediaPlayer to draw</param>
        public void DrawMediaPlayer(MediaPlayer player)
        {
            m_drawStateManagement.DrawPreamble();

            if (player.InternalBitmap == null)
                return;

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(player.InternalBitmap);
        }


        /// <summary>
        /// Draws a DrawingLayer on to another DrawingLayer
        /// </summary>
        /// <param name="drawingLayer">The DrawingLayer to draw</param>
        /// <param name="destinationRectangle">The destination rectangle</param>
        /// <param name="sourceRectangle">The source rectangle</param>
        /// <param name="opacity">The transparency level, 0 - 1</param>
        public void DrawLayer(DrawingLayer drawingLayer, RectangleF destinationRectangle, RectangleF sourceRectangle, float opacity)
        {
            m_drawStateManagement.DrawPreamble();

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(drawingLayer.D2DRenderTarget.InternalBitmap, 
                                                            destinationRectangle.InternalRectangleF, 
                                                            opacity, 
                                                            InterpolationMode.Linear, 
                                                            sourceRectangle.InternalRectangleF);
        }

        /// <summary>
        /// Draws a DrawingLayer on to another DrawingLayer
        /// </summary>
        /// <param name="drawingLayer">The DrawingLayer to draw</param>
        /// <param name="destinationRectangle">The destination rectangle</param>
        /// <param name="opacity">The transparency level, 0 - 1</param>
        public void DrawLayer(DrawingLayer drawingLayer, RectangleF destinationRectangle, float opacity)
        {
            m_drawStateManagement.DrawPreamble();

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(drawingLayer.D2DRenderTarget.InternalBitmap,
                                                            destinationRectangle.InternalRectangleF,
                                                            opacity,
                                                            InterpolationMode.Linear);
        }

        /// <summary>
        /// Draws a DrawingLayer on to another DrawingLayer
        /// </summary>
        /// <param name="drawingLayer">The DrawingLayer to draw</param>
        /// <param name="destinationRectangle">The destination rectangle</param>
        public void DrawLayer(DrawingLayer drawingLayer, RectangleF destinationRectangle)
        {
            m_drawStateManagement.DrawPreamble();

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(drawingLayer.D2DRenderTarget.InternalBitmap,
                                                            destinationRectangle.InternalRectangleF,
                                                            1.0f,
                                                            InterpolationMode.Linear);
        }

        /// <summary>
        /// Draws a DrawingLayer on to another DrawingLayer
        /// </summary>
        /// <param name="drawingLayer">The DrawingLayer to draw</param>
        public void DrawLayer(DrawingLayer drawingLayer)
        {
            m_drawStateManagement.DrawPreamble();

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(drawingLayer.D2DRenderTarget.InternalBitmap);
        }

        /// <summary>
        /// Fills an opacity mask on the DrawingLayer
        /// </summary>
        /// <param name="brush">The brush containing the content to fill</param>
        /// <param name="mask">The opacity mask</param>
        /// <param name="sourceRect">The source rectangle to use</param>
        /// <param name="destinationRect">The destination rectangle to use</param>
        public void FillOpacityMask(Brush brush, DrawingLayer mask, RectangleF destinationRect, RectangleF sourceRect)
        {
            FillOpacityMaskInternal(brush, mask.D2DRenderTarget.InternalBitmap, destinationRect, sourceRect);
        }

        /// <summary>
        /// Fills an opacity mask on the DrawingLayer
        /// </summary>
        /// <param name="brush">The brush containing the content to fill</param>
        /// <param name="mask">The opacity mask</param>
        /// <param name="sourceRect">The source rectangle to use</param>
        /// <param name="destinationRect">The destination rectangle to use</param>
        private void FillOpacityMaskInternal(Brush brush, SlimDX.Direct2D.Bitmap mask, RectangleF destinationRect, RectangleF sourceRect)
        {
            m_drawStateManagement.DrawPreamble();

            /* Save our state */
            PushState();

            /* Prepare our brush to be used */
            BrushHelper.PrepareBrush(brush, this, destinationRect, Matrix3x2.Identity, Matrix3x2.Identity);

            D2DRenderTarget.InternalRenderTarget.AntialiasMode = AntialiasMode.Aliased;

            D2DRenderTarget.InternalRenderTarget.FillOpacityMask(mask,
                                                                 brush.InternalBrush,
                                                                 OpacityMaskContent.Graphics,
                                                                 sourceRect.InternalRectangleF,
                                                                 destinationRect.InternalRectangleF);
            /* Restore our state */
            PopState();
        }

        /// <summary>
        /// Internal implementation to fill a rectangle
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="rectangle">The rectangle area</param>
        /// <param name="localTransform">The local transform</param>
        /// <param name="worldTransform">The world transform</param>
        private void FillRectangle(Brush brush, RectangleF rectangle, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();

            /* Save our state */
            PushState();

            /* Set our D2D render target to use the transform */
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            /* Prepare our brush to be used */
            BrushHelper.PrepareBrush(brush, this, rectangle, localTransform, worldTransform);

            D2DRenderTarget.InternalRenderTarget.FillRectangle(brush.InternalBrush, rectangle.InternalRectangleF);

            /* Restore our state */
            PopState();
        }

        /// <summary>
        /// Fills a rectangle on the DrawingLayer with the given brush
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="rectangle">The rectangle area</param>
        public void FillRectangle(Brush brush, RectangleF rectangle)
        {
            FillRectangle(brush, rectangle, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        /// <summary>
        /// Fills a rectangle on the DrawingLayer with the given brush
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="rectangle">The rectangle area</param>
        /// <param name="transform">The transformation to apply to the rectangle</param>
        public void FillRectangle(Brush brush, RectangleF rectangle, GeneralTransform transform)
        {
            var localTransform = transform.GetTransform();

            FillRectangle(brush, rectangle, Matrix3x2.Identity, localTransform);
        }

        /// <summary>
        /// Internal implementation to fill a rounded rectangle
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="rectangle">The rounded rectangle area</param>
        /// <param name="localTransform">The local transform</param>
        /// <param name="worldTransform">The world transform</param>
        private void FillRoundedRectangle(Brush brush, RoundedRectangleF rectangle, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();

            /* Save our state */
            PushState();

            /* Set our D2D render target to use the transform */
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            /* Prepare our brush to be used */
            BrushHelper.PrepareBrush(brush, this, rectangle.GetBounds(), localTransform, worldTransform);

            D2DRenderTarget.InternalRenderTarget.FillRoundedRectangle(brush.InternalBrush, rectangle.InternalRoundedRectangle);

            /* Restore our state */
            PopState();
        }

        /// <summary>
        /// Fills a rounded rectangle on the DrawingLayer with the given brush
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="rectangle">The rounded rectangle area</param>
        public void FillRoundedRectangle(Brush brush, RoundedRectangleF rectangle)
        {
            FillRoundedRectangle(brush, rectangle, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        /// <summary>
        /// Fills a rounded rectangle on the DrawingLayer with the given brush
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="rectangle">The rounded rectangle area</param>
        /// <param name="transform">The transformation to apply to the rounded rectangle</param>
        public void FillRoundedRectangle(Brush brush, RoundedRectangleF rectangle, GeneralTransform transform)
        {
            var localTransform = transform.GetTransform();

            FillRoundedRectangle(brush, rectangle, Matrix3x2.Identity, localTransform);
        }

        /// <summary>
        /// Internal implementation to fill an ellipse
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="ellipse">The ellipse area</param>
        /// <param name="localTransform">The local transform</param>
        /// <param name="worldTransform">The world transform</param>
        private void FillEllipse(Brush brush, Shapes.Ellipse ellipse, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();

            /* Save our state */
            PushState();

            /* Get the rectangular bounds of our ellipse */
            var bounds = ellipse.GetBounds();

            /* Set our D2D render target to use the transform */
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            /* Prepare our brush to be used */
            BrushHelper.PrepareBrush(brush, this, bounds, localTransform, worldTransform);

            D2DRenderTarget.InternalRenderTarget.FillEllipse(brush.InternalBrush,
                                                             ellipse.InternalEllipse);

            /* Restore our state */
            PopState();
        }

        /// <summary>
        /// Fills an ellipse on the DrawingLayer with the given brush
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="ellipse">The ellipse area</param>
        public void FillEllipse(Brush brush, Shapes.Ellipse ellipse)
        {
            FillEllipse(brush, ellipse, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        /// <summary>
        /// Fills an ellipse on the DrawingLayer with the given brush
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="ellipse">The ellipse area</param>
        /// <param name="transform">The transformation to apply to the ellipse</param>
        public void FillEllipse(Brush brush, Shapes.Ellipse ellipse, GeneralTransform transform)
        {
            var worldTransform = transform.GetTransform();

            FillEllipse(brush, ellipse, Matrix3x2.Identity, worldTransform);
        }

        /// <summary>
        /// Internal implementation to draw an ellipse
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="ellipse">The ellipse area</param>
        /// <param name="strokeWidth">The width of the stroke</param>
        /// <param name="localTransform">The local transform</param>
        /// <param name="worldTransform">The world transform</param>
        private void DrawEllipse(Brush brush, Shapes.Ellipse ellipse, float strokeWidth, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();

            /* Save our state */
            PushState();

            /* Get the rectangular bounds of our ellipse */
            var bounds = ellipse.GetBounds();

            /* Set our D2D render target to use the transform */
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            /* Prepare our brush to be used */
            BrushHelper.PrepareBrush(brush, this, bounds, localTransform, worldTransform);

            D2DRenderTarget.InternalRenderTarget.DrawEllipse(brush.InternalBrush,
                                                             ellipse.InternalEllipse,
                                                             strokeWidth);
            /* Restore our state */
            PopState();
        }

        /// <summary>
        /// Draws an ellipse on the DrawingLayer with the given brush
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="ellipse">The ellipse area</param>
        /// <param name="strokeWidth">The width of the stroke</param>
        public void DrawEllipse(Brush brush, Shapes.Ellipse ellipse, float strokeWidth)
        {
            DrawEllipse(brush, ellipse, strokeWidth, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        /// <summary>
        /// Draws an ellipse on the DrawingLayer with the given brush
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="ellipse">The ellipse area</param>
        /// <param name="strokeWidth">The width of the stroke</param>
        /// <param name="transform">The transformation to apply to the ellipse</param>
        public void DrawEllipse(Brush brush, Shapes.Ellipse ellipse, float strokeWidth, GeneralTransform transform)
        {
            var matrixTransform = transform.GetTransform();

            DrawEllipse(brush, ellipse, strokeWidth, Matrix3x2.Identity, matrixTransform);
        }

        private void DrawRectangle(Brush brush, RectangleF rect, float strokeWidth, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();

            /* Save our state */
            PushState();

            /* Set our D2D render target to use the transform */
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            /* Prepare our brush to be used */
            BrushHelper.PrepareBrush(brush,
                                     this,
                                     rect,
                                     localTransform, worldTransform);

            D2DRenderTarget.InternalRenderTarget.DrawRectangle(brush.InternalBrush, 
                                                               rect.InternalRectangleF,
                                                               strokeWidth);

            /* Restore our state */
            PopState();
        }

        public void DrawRectangle(Brush brush, RectangleF rect, float strokeWidth)
        {

            
            DrawRectangle(brush, rect, strokeWidth, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        public void DrawRectangle(Brush brush, RectangleF rect, float strokeWidth, GeneralTransform transform)
        {
            var matrixTransform = transform.GetTransform();

            DrawRectangle(brush, rect, strokeWidth, Matrix3x2.Identity, matrixTransform);
        }

        private void DrawRoundedRectangle(Brush brush, RoundedRectangleF rectangle, float strokeWidth, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();

            /* Save our state */
            PushState();

            /* Set our D2D render target to use the transform */
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            /* Prepare our brush to be used */
            BrushHelper.PrepareBrush(brush, this, rectangle.GetBounds(), localTransform, worldTransform);

            D2DRenderTarget.InternalRenderTarget.DrawRoundedRectangle(brush.InternalBrush, rectangle.InternalRoundedRectangle, strokeWidth);

            /* Restore our state */
            PopState();
        }

        public void DrawRoundedRectangle(Brush brush, RoundedRectangleF rectangle, float strokeWidth)
        {
            DrawRoundedRectangle(brush, rectangle, strokeWidth, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        public void DrawRoundedRectangle(Brush brush, RoundedRectangleF rectangle, float strokeWidth, GeneralTransform transform)
        {
            var localTransform = transform.GetTransform();

            DrawRoundedRectangle(brush, rectangle, strokeWidth, Matrix3x2.Identity, localTransform);
        }

        public void DrawGeometry(Brush brush, Shapes.Geometry geometry, float strokeWidth)
        {
            m_drawStateManagement.DrawPreamble();

            geometry.Draw(this, brush, strokeWidth);
        }

        public void FillGeometry(Brush brush, Shapes.Geometry geometry)
        {
            m_drawStateManagement.DrawPreamble();

            geometry.Fill(this, brush);
        }

        private SlimDX.DirectWrite.Factory m_directWriteFactory;
        private TextFormat m_textFormat;
        public void DrawText(Brush brush, string text, float x, float y)
        {
            DrawText(brush, text, x, y, 12);
        }

        public void DrawText(Brush brush, string text, float x, float y, float fontSize)
        {
            m_drawStateManagement.DrawPreamble();

            if (m_directWriteFactory == null)
            {
                m_directWriteFactory = new Factory(FactoryType.Isolated);
            }
            if (m_textFormat == null ||
                m_textFormat.FontSize != fontSize)
            {
                m_textFormat = new TextFormat(m_directWriteFactory, "Segoe UI", FontWeight.Bold, FontStyle.Normal, FontStretch.Normal, fontSize, "");
            }
            
            PushState();
            //var area2 = new RectangleF(0, 0, , );

            if (x > Width - 1 || y > Height - 1)
                return;

            var area = new RectangleF(x, y, Width, Height);

            var matrix = Matrix3x2.Identity;

            BrushHelper.PrepareBrush(brush, this, area, matrix, Matrix3x2.Identity);
 
            D2DRenderTarget.InternalRenderTarget.DrawText(text, 
                                                          m_textFormat,
                                                        area.InternalRectangleF,
                                                    brush.InternalBrush);
            

            PopState();
        }

        public void ApplyEffect(ShaderEffect effect, DrawingLayer output, bool clearOutput = true)
        {
            m_directCanvasFactory.ApplyEffect(effect, this, output, clearOutput);
        }

        public void ApplyEffect(ShaderEffect effect, DrawingLayer output, Rectangle targetRect, bool clearOutput = true)
        {
            m_directCanvasFactory.ApplyEffect(effect, this, output, targetRect, clearOutput);
        }

        public void CopyFromImage(Image sourceImage, Rectangle destRect)
        {
            sourceImage.CopyToDrawingLayer(this, destRect);
        }

        public void CopyFromImage(Image sourceImage)
        {
            sourceImage.CopyToDrawingLayer(this);
        }

        public virtual void Dispose()
        {
            if(SystemMemoryTexture != null)
            {
                SystemMemoryTexture.Dispose();
                SystemMemoryTexture = null;
            }

            if(m_d2DRenderTarget != null)
            {
                m_d2DRenderTarget.Dispose();
                m_d2DRenderTarget = null;
            }

            if (m_renderTargetTexture != null)
            {
                m_renderTargetTexture.Dispose();
                m_renderTargetTexture = null;
            }

            if (m_textFormat != null)
            {
                m_textFormat.Dispose();
                m_textFormat = null;
            }

            if(m_directWriteFactory != null)
            {
                m_directWriteFactory.Dispose();
                m_directWriteFactory = null;
            }

            foreach (var stateBlock in m_stateStack)
            {
                stateBlock.Dispose();
            }

            m_stateStack.Clear();
        }
    }
}
