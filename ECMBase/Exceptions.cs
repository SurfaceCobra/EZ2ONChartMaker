using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMBase
{
    public class NameNotFoundException : Exception
    {
        public NameNotFoundException(){ }

        public NameNotFoundException(string message)
            : base($"{message} 이름이 없습니다.")
        {}
    }

    public class ImageNotFoundException : Exception
    {
        public ImageNotFoundException() { }

        public ImageNotFoundException(string message)
            : base($"{message} 이미지 파일이 없습니다.")
        { }
    }

}
