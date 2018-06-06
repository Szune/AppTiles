﻿// performance of rich text boxes are.. suboptimal on my computer.
#define rtbLags

using System.ComponentModel;
using AppTiles.Tiles;
using System.Windows;
using System.Windows.Controls;
#if !rtbLags
using System.Windows.Documents;
#endif
using System.Windows.Input;

namespace AppTiles.Windows
{
    /// <summary>
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {
        private readonly NoteTile _tile;
        #if rtbLags
            private readonly TextBox _txtNote = new TextBox{AcceptsReturn = true, AcceptsTab = true, TextWrapping = TextWrapping.Wrap, VerticalScrollBarVisibility = ScrollBarVisibility.Auto};
        #else
            private readonly RichTextBox _txtNote = new RichTextBox {AcceptsReturn = true, AcceptsTab = true};
        #endif

        public NoteWindow(NoteTile tile)
        {
            InitializeComponent();
            MainGrid.Children.Add(_txtNote);
            _tile = tile;
            SetupTextBox();
        }

        private void SetupTextBox()
        {
            #if rtbLags
                _txtNote.Text = _tile.Note;
                _txtNote.Focus();
            #else
                _txtNote.AppendText(_tile.Note);
                _txtNote.Focus();
            #endif
            Grid.SetRow(_txtNote, 1);
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            Save();
            Close();
        }

        private void Save()
        {
            _tile.Note = GetText();
            Settings.SetChanged();
        }

        private void NoteWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
            {
                e.Handled = true;
                Save();
                Close();
            }
            else if (e.Key == Key.Escape)
            {
                AskToSave();
                Close();
            }

        }

        private void AskToSave()
        {
            if (_tile.Note == GetText())
            {
                return;
            }
            var result = MessageBox.Show("There are unsaved changes. Would you like to save them?",
                "Unsaved changes",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;
            Save();
        }

        private string GetText()
        {
        #if rtbLags
            return _txtNote.Text;
        #else
            return StringFromRichTextBox(_txtNote);
        #endif
        }

#if !rtbLags
        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart, 
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }
#endif
        private void NoteWindow_OnClosing(object sender, CancelEventArgs e)
        {
            AskToSave();
        }
    }
}
