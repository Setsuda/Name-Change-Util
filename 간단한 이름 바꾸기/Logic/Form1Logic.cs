using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 간단한_이름_바꾸기.Logic
{
    public class Form1Logic
    {


        /// <summary>
        /// 폴더 선택 버튼 클릭 처리
        /// </summary>
        /// <param name="changeName"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="currentFolder"></param>
        /// <returns></returns>
        public string FolderSelect(ChangeName changeName, object sender, EventArgs e, string currentFolder)
        {

            string rtn = currentFolder;

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "폴더를 선택해 주세요.";

            fbd.RootFolder = Environment.SpecialFolder.MyComputer;

            try
            {
                if (currentFolder != "") fbd.SelectedPath = currentFolder;
            }
            catch (Exception)
            {

            }

            if (fbd.ShowDialog(changeName) == DialogResult.OK)
            {
                rtn = fbd.SelectedPath;
            }

            return rtn;
        }


        /// <summary>
        /// 리스트 역순 재배치
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<string> ListSortChange(IList<string> list)
        {
            List<string> rtn = new List<string>();
            if (list == null) return rtn;
            for( int i= list.Count-1; i>= 0; i--)
            {
                rtn.Add(list[i]);
            }

            return rtn;
        }


        /// <summary>
        /// 폴더 혹은 파일의 존재 체크
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        public bool IsExists(string path, bool isFolder)
        {
            bool rtn = false;

            if (!isFolder) rtn = File.Exists(path);
            else rtn = Directory.Exists(path);

            return rtn;

        }

        
    }
}
