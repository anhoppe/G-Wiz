
namespace Gwiz.Core.Contract
{
    [Serializable]
    internal class UnknownTemplateParameterValue : Exception
    {
        public UnknownTemplateParameterValue()
        {
        }

        public UnknownTemplateParameterValue(string? message) : base(message)
        {
        }

        public UnknownTemplateParameterValue(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}