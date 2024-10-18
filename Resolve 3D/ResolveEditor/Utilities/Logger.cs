using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ResolveEditor.Utilities
{
    /// <summary>
    /// Represents the types of messages for logging: Info, Warning, and Error.
    /// </summary>
    enum MessageType
    {
        Info = 0x01, Warning = 0x02, Error = 0x04,
    }

    /// <summary>
    /// Represents a log message with details like time, type, file, caller, and line.
    /// </summary>
    class LogMessage
    {
        public DateTime Time { get; }
        public MessageType MessageType { get; }
        public string Message { get; }
        public string File { get; }
        public string Caller { get; }
        public int Line { get; }
        public string MetaData => $"{File}: {Caller} ({Line})";

        /// <summary>
        /// Initializes a new instance of LogMessage with the specified details.
        /// </summary>
        public LogMessage(MessageType messageType, string message, string file, string caller, int line)
        {
            Time = DateTime.Now;
            MessageType = messageType;
            Message = message;
            File = Path.GetFileName(file);
            Caller = caller;
            Line = line;
        }
    }

    /// <summary>
    /// A static class responsible for logging messages with filtering and UI update support.
    /// </summary>
    static class Logger
    {
        private static int _messageFilter =(int)(MessageType.Info | MessageType.Warning | MessageType.Error);
        private readonly static ObservableCollection<LogMessage> _messages = new ObservableCollection<LogMessage>();

        /// <summary>
        /// Gets the collection of all log messages.
        /// </summary>
        public static ReadOnlyObservableCollection<LogMessage> Messages { get; } = new ReadOnlyObservableCollection<LogMessage>(_messages);

        /// <summary>
        /// Gets the collection view source of filtered messages based on the message filter.
        /// </summary>
        public static CollectionViewSource FilteredMessages 
        { get; } = new CollectionViewSource() { Source = Messages };

        /// <summary>
        /// Logs a new message to the UI, adding it to the internal collection.
        /// </summary>
        /// <param name="type">The type of the message (Info, Warning, or Error).</param>
        /// <param name="msg">The message content.</param>
        /// <param name="file">The file where the log was called.</param>
        /// <param name="caller">The calling method.</param>
        /// <param name="line">The line number in the file.</param>
        public static async void Log(MessageType type, string msg,
            [CallerFilePath]string file = "",
            [CallerMemberName]string caller = "",
            [CallerLineNumber]int line=0)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _messages.Add(new LogMessage(type, msg, file, caller, line));
            }));
        }

        /// <summary>
        /// Clears all log messages.
        /// </summary>
        public static async void Clear()
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _messages.Clear();
            }));
        }

        /// <summary>
        /// Sets the filter for which message types should be displayed.
        /// </summary>
        /// <param name="mask">The bitmask of message types to include (Info, Warning, Error).</param>
        public static void SetMessageFilter(int mask)
        {
            _messageFilter = mask;
            FilteredMessages.View.Refresh();
        }

        /// <summary>
        /// Static constructor that initializes the message filtering system.
        /// </summary>
        static Logger()
        {
            FilteredMessages.Filter += (s, e) =>
            {
                var type = (int)(e.Item as LogMessage).MessageType;
                e.Accepted = (type & _messageFilter) != 0;
            };
        }
    }
}
