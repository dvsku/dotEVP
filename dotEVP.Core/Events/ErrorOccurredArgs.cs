using System;
using System.Collections.Generic;
using System.Text;

namespace dotEVP.Core.Events {
    public class ErrorOccurredArgs : EventArgs {
        public string Operation { get; }
        public string Text { get; }

        public ErrorOccurredArgs(string operation, string text) {
            Operation = operation;
            Text = text;
        }
    }
}