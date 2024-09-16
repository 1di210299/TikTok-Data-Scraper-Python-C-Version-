using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace WebScrapingWinForms
{
    public class TextBoxLoggerProvider : ILoggerProvider
    {
        private readonly TextBox _textBox;

        public TextBoxLoggerProvider(TextBox textBox)
        {
            _textBox = textBox;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TextBoxLogger(_textBox);
        }

        public void Dispose() { }
    }

    public class TextBoxLogger : ILogger
    {
        private readonly TextBox _textBox;

        public TextBoxLogger(TextBox textBox)
        {
            _textBox = textBox;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"[{DateTime.Now}] [{logLevel}] {message}";
            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            _textBox.Invoke((MethodInvoker)delegate {
                _textBox.AppendText(message + Environment.NewLine);
            });
        }
    }
}