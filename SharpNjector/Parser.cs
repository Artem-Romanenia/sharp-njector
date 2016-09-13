using System;
using SharpNjector.Exceptions;
using SharpNjector.Properties;

namespace SharpNjector
{
    public static class Parser
    {
        public static void Parse(string input, Action<BlockType, int, int> onBlockParsed)
        {
            var njectUsingKeywordLength = Resources.NjectUsingKeyWord.Length;
            var njectKeywordLength = Resources.NjectKeyWord.Length;

            var blockStartPosition = 0;
            for (var i = 0; i < input.Length; i++)
            {
                if (input.Length >= i + njectUsingKeywordLength && input.Substring(i, njectUsingKeywordLength) == Resources.NjectUsingKeyWord)
                {
                    onBlockParsed(BlockType.Text, blockStartPosition, i - blockStartPosition);
                    blockStartPosition = i;

                    i += njectUsingKeywordLength;

                    if (input[i++] != '(')
                        throw new ParsingException();

                    onBlockParsed(BlockType.NjectUsingLeft, blockStartPosition, i - blockStartPosition);
                    blockStartPosition = i;

                    var depth = 1;
                    do
                    {
                        if (input[i] == '(')
                            depth++;
                        else if (input[i] == ')')
                            depth--;

                        if (depth > 0)
                            i++;
                        else
                        {
                            onBlockParsed(BlockType.Code, blockStartPosition, i - blockStartPosition);
                            blockStartPosition = i;
                        }
                    } while (depth > 0 && i < input.Length);

                    if (depth > 0)
                        throw new ParsingException();

                    if (input.Substring(i + 1, Environment.NewLine.Length) == Environment.NewLine)
                        i += Environment.NewLine.Length;

                    onBlockParsed(BlockType.NjectUsingRight, blockStartPosition, i - blockStartPosition + 1);
                    blockStartPosition = i + 1;
                }
                else if (input.Length >= i + njectKeywordLength &&
                         input.Substring(i, njectKeywordLength) == Resources.NjectKeyWord)
                {
                    onBlockParsed(BlockType.Text, blockStartPosition, i - blockStartPosition);
                    blockStartPosition = i;

                    i += njectKeywordLength;

                    if (input[i++] != '(')
                        throw new ParsingException();

                    onBlockParsed(BlockType.NjectLeft, blockStartPosition, i - blockStartPosition);
                    blockStartPosition = i;

                    int depth = 1;
                    do
                    {
                        if (input[i] == '(')
                            depth++;
                        else if (input[i] == ')')
                            depth--;

                        if (depth > 0)
                            i++;
                        else
                        {
                            onBlockParsed(BlockType.Code, blockStartPosition, i - blockStartPosition);
                            blockStartPosition = i;
                        }
                    } while (depth > 0 && i < input.Length);

                    if (depth > 0)
                        throw new ParsingException();

                    onBlockParsed(BlockType.NjectUsingRight, blockStartPosition, i - blockStartPosition + 1);
                    blockStartPosition = i + 1;
                }
            }

            onBlockParsed(BlockType.Text, blockStartPosition, input.Length - blockStartPosition);
        }

        public enum BlockType
        {
            NjectLeft = 0,
            NjectRight = 1,
            NjectUsingLeft = 2,
            NjectUsingRight = 3,
            Text = 4,
            Code = 5
        }
    }
}
