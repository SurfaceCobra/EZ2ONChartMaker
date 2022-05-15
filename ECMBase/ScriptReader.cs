using System.Collections;
using System.Text;

namespace ECMBase
{

    public class ScriptReader : IEnumerable<Ranged<string>>
    {
        char[] Src;
        int index;

        public ScriptReader(string src)
        {
            this.Src = src.ToCharArray();
            index = 0;
        }

        public ScriptReader(char[] src, int index)
        {
            this.Src = src;
            index = 0;
        }

        public bool TryReadNext(out Ranged<string> output)
        {
            output = null;


            //index 는 시작지점, curr은 길이

            int curr = 0;
            int currIndex() => index + curr;
            char currWord() => Src[currIndex()];

            if (currIndex() >= Src.Length)
            {
                return false;
            }

            //초기 IgnoreWord 제거
            while (true)
            {
                if (currIndex() >= Src.Length)
                {
                    return false;
                }
                else if (IgnoreWords.Contains(currWord()))
                {
                    curr++;
                }
                else
                {
                    break;
                }
            }

            //시작이 UsedWords일경우 바로 반환
            if (UsedWords.Contains(currWord()))
            {
                output = new Ranged<string>(currWord().ToString(), index..currIndex());
                index += curr + 1;

                return true;
            }



            int beginIndex = currIndex();


            //시작이 "일경우 다음 "까지 쭉 달림
            if (currWord() == '\"')
            {
                curr++;
                while (currWord() != '\"')
                {
                    curr++;
                }
                curr++;

                output = new Ranged<string>(new(Src[beginIndex..currIndex()]), index..currIndex());

                index += curr;
                return true;
            }


            curr++;




            while (true)
            {
                if (currIndex() >= Src.Length)
                {
                    if (beginIndex == currIndex())
                    {
                        return false;
                    }
                    else
                    {
                        output = new Ranged<string>(new(Src[beginIndex..currIndex()]), index..currIndex());
                        index += curr;
                        return true;
                    }
                }
                else if (UsedWords.Contains(currWord()) || IgnoreWords.Contains(currWord()))
                {
                    output = new Ranged<string>(new(Src[beginIndex..currIndex()]), index..currIndex());
                    index += curr;
                    return true;
                }
                curr++;
            }
        }

        public string ReadNext()
        {
            TryReadNext(out Ranged<string> str);
            return str;
        }

        public IEnumerator<Ranged<string>> GetEnumerator()
        {
            while (this.TryReadNext(out Ranged<string> str))
            {
                yield return str;
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        char[] UsedWords = new char[]
        {
            '.',
            ',',
            '<',
            '>',
            '/',
            '?',
            ':',
            ';',
            '\'',
            '[',
            ']',
            '{',
            '}',
            '(',
            ')',
            '*',
            '&',
            '^',
            '%',
            '$',
            '#',
            '@',
            '!',
            '~',
            '`',
            '-',
            '=',
            '+',
//            '.',
        };
        char[] IgnoreWords = new char[]
        {
            ' ',
            '\r',
            '\n',
            '\t',
        };
    }


    public class FunctionReader : IEnumerable<string>
    {
        public ScriptReader reader;

        public enum Bracket
        {
            Small,
            Medium,
            Big,
            Comparer
        }

        public IEnumerator<string> GetEnumerator()
        {
            Stack<Bracket> stack = new Stack<Bracket>(32);
            foreach (string text in this.reader)
            {
                switch (text)
                {
                    case "(":
                        stack.Push(Bracket.Small);
                        goto end;
                    case "{":
                        stack.Push(Bracket.Medium);
                        goto end;
                    case "[":
                        stack.Push(Bracket.Big);
                        goto end;
                    case "<":
                        stack.Push(Bracket.Comparer);
                        goto end;

                    end:
                        yield return text;
                        break;

                    case ")":
                        if (stack.Peek() != Bracket.Small)
                            throw new Exception(stack.Peek() + "괄호가 더닫힘");
                        stack.Pop();
                        break;
                    case "}":
                        if (stack.Peek() != Bracket.Medium)
                            throw new Exception(stack.Peek() + "괄호가 더닫힘");
                        stack.Pop();
                        break;
                    case "]":
                        if (stack.Peek() != Bracket.Big)
                            throw new Exception(stack.Peek() + "괄호가 더닫힘");
                        stack.Pop();
                        break;
                    case ">":
                        if (stack.Peek() != Bracket.Comparer)
                            throw new Exception(stack.Peek() + "괄호가 더닫힘");
                        stack.Pop();
                        break;
                }

                if (stack.Count == 0)
                    yield break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }


    public class ScriptBlockReader
    {
        ///작동방식
        ///쭉 읽다가 ;나오면 블럭 반환
        ///{ 나오면 닫힐때까지 읽고 블럭 반환
        ///

        public ScriptBlockReader(ScriptReader reader)
        {
            this.reader = reader;
        }

        ScriptReader reader;

        public ScriptBlock.Block Read()
        {
            return ReadSingleBlock(Bracket.Shape.EOF);
        }



        private ScriptBlock.Block ReadSingleBlock(Bracket.Shape opener)
        {
            ScriptBlock.Block currentblock = new ScriptBlock.Block(opener);
            ScriptBlock.Block eolBlock = null;
            bool eolState = false;
            foreach (var str in reader)
            {

                if (str.o.Length == 1 && Bracket.Groups.TryGetValue(str.o[0], out (Bracket.Shape shape, Bracket.Location loc) val))
                {
                    switch (val.loc)
                    {
                        case Bracket.Location.Open:
                        open:
                            var block = ReadSingleBlock(val.shape);

                            if (block.shape == Bracket.Shape.EOL)
                            {
                                if (!eolState)
                                {
                                    eolBlock = new ScriptBlock.Block(val.shape);
                                }
                                eolState = true;
                                eolBlock.Add(block);
                                goto open;
                            }

                            if (eolState)
                            {
                                eolState = false;
                                block.ChangeShape(Bracket.Shape.EOL);
                                eolBlock.Add(block);
                                currentblock.Add(eolBlock);
                            }
                            else
                            {
                                currentblock.Add(block);
                            }

                            break;

                        case Bracket.Location.Close:
                        close:
                            if (val.shape == opener)
                                return currentblock;
                            throw new Exception();

                        case Bracket.Location.Both:
                            if (opener == val.shape)
                                goto close;
                            else
                                goto open;

                        case Bracket.Location.CloseOnly:
                            currentblock.ChangeShape(val.shape);
                            return currentblock;

                        default:
                            throw new Exception();
                    }
                }
                else
                {
                    currentblock.Add(new ScriptBlock.Value(str));
                }

            }
            //eof
            if (opener == Bracket.Shape.EOF) return currentblock;
            throw new Exception();


        }
    }

    public interface ScriptBlock : IRangeInside
    {

        public class Block : ScriptBlock, IEnumerable<ScriptBlock>
        {
            public Block(Bracket.Shape shape)
            {
                this._shape = shape;
            }
            public void Add(ScriptBlock item)
            {
                if (item is ScriptBlock.Block block)
                {
                    if (block.values.Count > 0)
                        values.Add(item);
                }
                else
                {
                    values.Add(item);
                }
            }
            public void ChangeShape(Bracket.Shape shape)
            {
                this._shape = shape;
            }

            List<ScriptBlock> values = new List<ScriptBlock>();

            Bracket.Shape _shape;
            public Bracket.Shape shape => _shape;

            public Range range => Ranged.Merge(values.ConvertAll(x => x.range));

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                string s = Bracket.StringOf((shape, Bracket.Location.Open));
                if (s.Length > 0)
                {
                    sb.AppendLine();
                    sb.Append(s);
                    sb.AppendLine();
                }

                foreach (var b in values)
                {
                    switch (b)
                    {
                        case ScriptBlock.Block bb:
                            sb.Append(bb.ToString());
                            break;
                        case ScriptBlock.Value vb:
                            sb.Append(vb.ToString());
                            sb.Append(" ");
                            break;
                    }
                }
                sb.Append(Bracket.StringOf((shape, Bracket.Location.Close)));
                sb.AppendLine();
                return sb.ToString();
            }

            public IEnumerator<ScriptBlock> GetEnumerator() => this.values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
        public class Value : ScriptBlock
        {
            public Value(Ranged<string> value)
            {
                this.value = value;
            }

            public Ranged<string> value;

            public Range range => value.range;

            public Ranged<string> ToRangedString()
            {
                return this.value;
            }
            public override string ToString()
            {
                return this.value;
            }
        }
    }



    public static class Bracket
    {
        public enum Shape
        {
            Null,
            None,
            Small,
            Medium,
            Big,
            Comparer,
            String,
            Char,
            EOL,
            EOF
        }

        public enum Location
        {
            Null,
            None,
            Open,
            Close,
            CloseOnly,
            Both
        }

        public static Dictionary<char, (Shape, Location)> Groups = new()
        {
            ['('] = (Shape.Small, Location.Open),
            [')'] = (Shape.Small, Location.Close),
            ['{'] = (Shape.Medium, Location.Open),
            ['}'] = (Shape.Medium, Location.Close),
            ['['] = (Shape.Big, Location.Open),
            [']'] = (Shape.Big, Location.Close),
            ['<'] = (Shape.Comparer, Location.Open),
            ['>'] = (Shape.Comparer, Location.Close),
            [';'] = (Shape.EOL, Location.CloseOnly),
            ['"'] = (Shape.String, Location.Both),
            ['\''] = (Shape.Char, Location.Both),
            [(char)0x00] = (Shape.EOF, Location.CloseOnly),
        };

        public static string StringOf((Shape, Location) val)
        {
            switch (val.Item1)
            {
                case Shape.Small:
                    switch (val.Item2)
                    {
                        case Location.Open:
                            return "(";
                        case Location.Close:
                            return ")";
                        default:
                            throw new Exception();
                    }

                case Shape.Medium:
                    switch (val.Item2)
                    {
                        case Location.Open:
                            return "{";
                        case Location.Close:
                            return "}";
                        default:
                            throw new Exception();
                    }

                case Shape.Big:
                    switch (val.Item2)
                    {
                        case Location.Open:
                            return "[";
                        case Location.Close:
                            return "]";
                        default:
                            throw new Exception();
                    }

                case Shape.Comparer:
                    switch (val.Item2)
                    {
                        case Location.Open:
                            return "<";
                        case Location.Close:
                            return ">";
                        default:
                            throw new Exception();
                    }

                case Shape.EOL:
                    switch (val.Item2)
                    {
                        case Location.Open:
                            return "";
                        case Location.Close:
                        case Location.CloseOnly:
                            return ";";
                        default:
                            throw new Exception();
                    }

                case Shape.String:
                    return "\"";

                case Shape.Char:
                    return "\'";

                case Shape.EOF:
                    switch (val.Item2)
                    {
                        case Location.Open:
                            return "";
                        case Location.Close:
                        case Location.CloseOnly:
                            return "#EOF#";
                        default:
                            throw new Exception();
                    }

                default:
                    return "#NULL#";
            }
        }
    }

}
