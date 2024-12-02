using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    
    public class InsertTextCommand : ICommand
    {
        private readonly string text;
        private readonly int position;
        private readonly TextEditor textEditor;

        public InsertTextCommand(TextEditor textEditor, string text, int position)
        {
            this.textEditor = textEditor;
            this.text = text;
            this.position = position;
        }

        public void Execute()
        {
            textEditor.InsertText(text, position);
        }

        public void Undo()
        {
            textEditor.DeleteText(position, text.Length);
        }
    }

    
    public class DeleteTextCommand : ICommand
    {
        private readonly int position;
        private readonly int length;
        private readonly string deletedText;
        private readonly TextEditor textEditor;

        public DeleteTextCommand(TextEditor textEditor, int position, int length)
        {
            this.textEditor = textEditor;
            this.position = position;
            this.length = length;
            deletedText = textEditor.GetText(this.position, length);
        }

        public void Execute()
        {
            textEditor.DeleteText(position, length);
        }

        public void Undo()
        {
            textEditor.InsertText(deletedText, position);
        }
    }


    
    public class TextEditor
    {
        private string text = "";

        public string Text { get => text; }


        public void InsertText(string text, int position)
        {
            if (position > this.text.Length)
            {
                this.text += text;
            }
            else
            {
                this.text = this.text.Insert(position, text);
            }
        }

        public void DeleteText(int position, int length)
        {
            if (position + length > this.text.Length)
            {
                this.text = this.text.Remove(position); 
            }
            else
            {
                this.text = this.text.Remove(position, length);
            }

        }

        public string GetText(int position, int length)
        {
            return text.Substring(position, length);
        }

    }

    public class CommandHistory
    {
        private readonly List<ICommand> commands = new List<ICommand>();

        public void Push(ICommand command)
        {
            commands.Add(command);
        }

        public void Undo()
        {
            if (commands.Count > 0)
            {
                ICommand command = commands[commands.Count - 1];
                command.Undo();
                commands.RemoveAt(commands.Count - 1);
            }
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            TextEditor editor = new TextEditor();
            CommandHistory history = new CommandHistory();
            // Добавление текста
            ICommand insertCommand1 = new InsertTextCommand(editor, "Hello", 0);
            insertCommand1.Execute();
            history.Push(insertCommand1);
            Console.WriteLine($"Text: {editor.Text}");  // Вывод: Hello
            // Добавляем ещё текст
            ICommand insertCommand2 = new InsertTextCommand(editor, " World!", 6);
            insertCommand2.Execute();
            history.Push(insertCommand2);
            Console.WriteLine($"Text: {editor.Text}"); // Вывод: Hello World!
            // Удаление текста
            ICommand deleteCommand = new DeleteTextCommand(editor, 6, 6);
            deleteCommand.Execute();
            history.Push(deleteCommand);
            Console.WriteLine($"Text: {editor.Text}"); // Вывод: Hello
            // Отмена последней операции
            history.Undo();
            Console.WriteLine($"Text: {editor.Text}"); // Вывод: Hello World!
            //Еще одна отмена
            history.Undo();
            Console.WriteLine($"Text: {editor.Text}"); // Вывод: Hello

            
        }
    }

}
