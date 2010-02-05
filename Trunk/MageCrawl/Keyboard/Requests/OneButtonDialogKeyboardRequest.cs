using Magecrawl.GameUI.Dialogs;

namespace Magecrawl.Keyboard.Requests
{
    struct OneButtonDialogKeyboardRequest
    {
        public OneButtonDialogKeyboardRequest(string text, OnOneButtonComplete completionDelegate)
        {
            Text = text;
            CompletionDelegate = completionDelegate;
        }

        public string Text;
        public OnOneButtonComplete CompletionDelegate;
    }
}
