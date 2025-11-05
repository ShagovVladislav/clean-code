using System;
using System.Collections.Generic;

namespace Markdown
{
    public class Tokenizer
    {
        private readonly string _text;
        private int _position;

        private static readonly HashSet<char> Escapable = new() { '\\', '#', '_' };
        private static readonly HashSet<char> StopChars = new() { '#', '_', '\\', '\n' };

        public Tokenizer(string text)
        {
            _text = text.Replace("\r", "");
            _position = 0;
        }

        private char Current => _position < _text.Length ? _text[_position] : '\0';
        private char Peek(int offset = 1)
        {
            int pos = _position + offset;
            return pos < _text.Length ? _text[pos] : '\0';
        }

        private bool HasMore => _position < _text.Length;

        private void Advance(int count = 1) => _position += count;

        private static bool IsEscapable(char c) => Escapable.Contains(c);

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (HasMore)
            {
                if (TryReadEscape(tokens)) continue;
                if (TryReadHeading(tokens)) continue;
                if (TryReadFormatting(tokens)) continue;
                if (TryReadNewLine(tokens)) continue;

                TryReadText(tokens);
            }

            tokens.Add(new Token(TokenType.EndOfFile));
            return tokens;
        }
        
        private bool TryReadEscape(List<Token> tokens)
        {
            if (Current != '\\') return false;

            char next = Peek();
            if (next != '\0' && IsEscapable(next))
            {
                Advance();
                tokens.Add(new Token(TokenType.Text, next.ToString()));
                Advance();
            }
            else
            {
                Advance();
                tokens.Add(new Token(TokenType.Text, "\\"));
            }
            return true;
        }

        private bool TryReadHeading(List<Token> tokens)
        {
            if (Current != '#') return false;

            int start = _position;
            while (Current == '#') Advance();
            string hashes = _text[start.._position];

            tokens.Add(new Token(TokenType.Heading, hashes));

            if (Current == ' ') Advance();
            return true;
        }

        private bool TryReadFormatting(List<Token> tokens)
        {
            if (Current == '_' && Peek() == '_')
            {
                Advance(2);
                tokens.Add(new Token(TokenType.BoldStart, "__"));
                return true;
            }
            if (Current == '_')
            {
                Advance();
                tokens.Add(new Token(TokenType.ItalicStart, "_"));
                return true;
            }

            return false;
        }

        private bool TryReadNewLine(List<Token> tokens)
        {
            if (Current != '\n') return false;

            Advance();
            tokens.Add(new Token(TokenType.NewLine, "\n"));
            return true;
        }

        private bool TryReadText(List<Token> tokens)
        {
            int start = _position;
            while (HasMore && !StopChars.Contains(Current))
                Advance();

            if (_position > start)
            {
                string text = _text[start.._position];
                tokens.Add(new Token(TokenType.Text, text));
                return true;
            }

            return false;
        }
    }
}
