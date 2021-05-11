using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursach
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            String expressionText = expressionTextBox.Text.ToString();
            if (CheckExpressionText(expressionText))
            {
                Queue<Char> result = new Parser().getPostFixEx(expressionText);
                String textResult = "";
                while (true)
                {
                    if (result.Count != 0)
                    {
                        textResult += Convert.ToString(result.Dequeue());
                    }
                    else break;
                }
                resultTextBox.Text = textResult;
                historyRichTextBox.Text += textResult + "\n";
            }
            else
                MessageBox.Show("Ошибка в выражении!");

        }

        public bool CheckExpressionText(String text)
        {
            if (text.Length <= 2)
            {
                errorsRichTextBox.Text += "Слишком короткое выражение!\n";
                return false;
            }
            if(Regex.IsMatch(text, @"^[=!@|,.&?^%$]+$") && text.All(c => Char.IsLetterOrDigit(c) || c == '_'))
            {
                errorsRichTextBox.Text += "Выражение содержит недопустимые символы!\n";
                return false;
            }
            else if (text.Contains("("))
            {
                int count1 = 0, count2 = 0;
                foreach (Char c in text)
                {
                    if (c.Equals('(')) count1 += 1;
                    if (c.Equals(')')) count2 += 1;
                }
                if (count1 > count2)
                {
                    errorsRichTextBox.Text += "Нехватает закрывающейся скобки\n";
                    return false;
                }
                else if (count1 < count2)
                {
                    errorsRichTextBox.Text += "Нехватает открывающейся скобки\n";
                    return false;
                }
                else return true;
            }
            else return true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            expressionTextBox.Text = "";
            resultTextBox.Text = "";
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            errorsRichTextBox.Text = "";
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            historyRichTextBox.Text = "";
        }
    }
    class Parser
    {
        Queue<Char> operands = new Queue<Char>();
        Stack<Char> operators = new Stack<Char>();

        public Queue<Char> getPostFixEx(string str)//публичный метод класса Parser
        {
            foreach (char c in str)
            {
                if (c.Equals('(')) push(c);
                if (c.Equals(')')) pop();
                if (Char.IsLetter(c) || Char.IsDigit(c)) addQueue(c);
                if(c.Equals('+') || c.Equals('-'))
                {
                    if (operators.Count != 0)
                    {
                        Char peek = Convert.ToChar(operators.Peek());
                        if (peek.Equals('('))
                        {
                            push(c);
                        }

                        else if (peek.Equals('/') || peek.Equals('*'))
                        {
                            pop();
                            push(c);
                        }
                        else
                        {
                            addQueue(operators.Pop());
                            push(c);
                        }
                    }else push(c);
                }
                if (c.Equals('*') || c.Equals('/'))
                {
                    if (operators.Count != 0)
                    {
                        Char peek = Convert.ToChar(operators.Peek());
                        if (peek.Equals('*') || peek.Equals('/'))
                        {
                            pop();
                        }
                    }
                    push(c);
                }

            }
            if (operators.Count != 0)
            {
                bool b = true;
                while (b)
                {
                    if (operators.Count != 0)
                    {
                        Char peek = Convert.ToChar(operators.Peek());
                        if(!peek.Equals('('))
                            operands.Enqueue(Convert.ToChar(operators.Pop()));
                    }
                    else
                        b = false;
                }
            }
                return operands;
        }
        public void pop()
        {
            while (true)
            {
                if (operators.Count != 0)
                {
                    Char c = Convert.ToChar(operators.Pop());
                    if (c.Equals('('))
                    {
                        break;
                    }
                    else
                    {
                        operands.Enqueue(c);
                    }
                }
                else break;
            }
        }

        public void addQueue(Char item)
        {
            operands.Enqueue(item);
        }

        public void push(Char item)
        {
            operators.Push(item);
        }
    }
}
