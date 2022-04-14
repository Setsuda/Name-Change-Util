using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 간단한_이름_바꾸기.Models
{
    [Serializable]
    public class ConfigData
    {
        //  설정 파일 형식
        /// <summary>위치 </summary>
        public List<string> lstFolder = new List<string>();
        /// <summary>파일 형식 </summary>
        public string sFileExt = "";
        /// <summary>파일 형식 리스트 </summary>
        public List<string> lstFileExt = new List<string>();
        /// <summary>하위 폴더 적용 판단 </summary>
        public bool isSubFolder = false;

        /// <summary>확장자 변경 적용 판단 </summary>
        public bool isChangeExt = false;
        /// <summary>확장자 변경 이력 </summary>
        public List<string> lstChangeExt = new List<string>();

        /// <summary>머릿말 사용 판단</summary>
        public bool isHead = false;
        /// <summary>머릿말</summary>
        public List<string> lstHead = new List<string>();

        /// <summary>꼬릿말 사용 판단</summary>
        public bool isTail = false;
        /// <summary>꼬릿말</summary>
        public List<string> lstTail = new List<string>();

        /// <summary>카운트 사용판단 </summary>
        public bool isCount = false;
        /// <summary>카운트 위치 판단 </summary>
        public bool isCountBack = false;
        /// <summary>카운트 자릿수 </summary>
        public List<string> lstCountNumber = new List<string>();
        /// <summary>카운트 머릿말 </summary>
        public List<string> lstCountHead = new List<string>();
        /// <summary>카운트 꼬릿말 </summary>
        public List<string> lstCountTail = new List<string>();

        /// <summary>구간 위치 변경 사용판단 </summary>
        public bool isPositionChange = false;
        /// <summary>구간 위치 변경 위치판단 </summary>
        public bool isPositionChangeBack = false;
        /// <summary>구간 위치 변경 단어시작  </summary>
        public List<string> lstPositionChangeStart = new List<string>();
        /// <summary>구간 위치 변경 단어끝</summary>
        public List<string> lstPositionChangeEnd = new List<string>();

        /// <summary>변경 전 문자</summary>
        public List<string> lstBeforeText = new List<string>();
        /// <summary>변경 후 내용</summary>
        public List<string> lstAfterText = new List<string>();

        
        /// <summary>구간 위치 변경 머리말 한정 판단</summary>
        public bool isPositionChangeFirst = false;
        /// <summary>구간 위치 변경 꼬리말 한정 판단</summary>
        public bool isPositionChangeLast = false;

        /// <summary>
        /// ConfigData 콘스트럭터
        /// </summary>
        public ConfigData()
        {
        }
    }
}
