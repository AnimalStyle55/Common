using NLog.LayoutRenderers;
using NLog.LayoutRenderers.Wrappers;

namespace Common.Logging.NLog.Renderers
{
    [LayoutRenderer("truncateLeft")]
    [AmbientProperty("Length")]
    internal class TruncateLeftRenderer : WrapperLayoutRendererBase
    {
        [System.ComponentModel.DefaultValue(10)]
        public int Length { get; set; }

        public TruncateLeftRenderer()
        {
            Length = 10;
        }

        protected override string Transform(string text)
        {
            if (text.Length > Length)
            {
                return text.Substring(text.Length - Length);
            }
            else if (text.Length < Length)
            {
                return text.PadLeft(Length);
            }
            else
            {
                return text;
            }
        }
    }
}