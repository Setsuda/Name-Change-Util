using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 간단한_이름_바꾸기.Models
{
    [Serializable]
    public class MacroData
    {
        //  설정 파일 형식
        /// <summary>위치 </summary>
        public string Folder = "";
        /// <summary>파일 형식 </summary>
        public string sFileExt = "";
        /// <summary>하위 폴더 적용 판단 </summary>
        public bool isSubFolder = false;

        /// <summary>확장자 변경 적용 판단 </summary>
        public bool isChangeExt = false;
        /// <summary>확장자 변경 </summary>
        public string ChangeExt = "";

        /// <summary>머릿말 사용 판단</summary>
        public bool isHead = false;
        /// <summary>머릿말</summary>
        public string Head = "";

        /// <summary>꼬릿말 사용 판단</summary>
        public bool isTail = false;
        /// <summary>꼬릿말</summary>
        public string Tail = "";

        /// <summary>카운트 사용판단 </summary>
        public bool isCount = false;
        /// <summary>카운트 위치 판단 </summary>
        public bool isCountBack = false;
        /// <summary>카운트 자릿수 </summary>
        public string CountNumber = "";
        /// <summary>카운트 머릿말 </summary>
        public string CountHead = "";
        /// <summary>카운트 꼬릿말 </summary>
        public string CountTail = "";

        /// <summary>구간 위치 변경 사용판단 </summary>
        public bool isPositionChange = false;
        /// <summary>구간 위치 변경 위치판단 </summary>
        public bool isPositionChangeBack = false;
        /// <summary>구간 위치 변경 단어시작  </summary>
        public string PositionChangeStart = "";
        /// <summary>구간 위치 변경 단어끝</summary>
        public string PositionChangeEnd = "";
        /// <summary>구간 위치 변경 머리말 한정 판단</summary>
        public bool isPositionChangeFirst = false;
        /// <summary>구간 위치 변경 꼬리말 한정 판단</summary>
        public bool isPositionChangeLast = false;


        /// <summary>문자 변경 사용판단</summary>
        public bool isTextChange = false;
        /// <summary>변경 전 문자</summary>
        public string BeforeText = "";
        /// <summary>변경 후 내용</summary>
        public string AfterText = "";

    }
}
