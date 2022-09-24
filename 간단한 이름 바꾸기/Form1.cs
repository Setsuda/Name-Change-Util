using System;
using System.Collections.Generic;
//using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using 간단한_이름_바꾸기.Models;

namespace 간단한_이름_바꾸기
{

    public partial class ChangeName : Form
    {
        public Log.ILog Log = new Log.Log();
        public static ChangeName instance;

        //  설정 파일 이름
        private const string CONFIG_NAME = "config.ini";

        private const string DATA_NAME = "CONFIGDATA";
        private const string TABLE_NAME = "CONFIGDATA_TBL";
        private const string COLUMN_PATH = "sPath";
        private const string COLUMN_FILEEXT = "sFileExt";
        private const string COLUMN_LIST_FILEEXT = "lstFileExt";
        private const string COLUMN_ISSUBFOLDERS = "isSubFolders";
        private const string COLUMN_BEFORE_TEXT = "sBeforeText";
        private const string COLUMN_AFTER_TEXT = "sAfterText";



        

        //  설정파일 
        private ConfigData ConfigData { get; set; }
        
        //  캔슬 플러그
        private static bool isCancel = false;

        //  캔슬대비 이름 격납고
        public class OriFileNameToChanged
        {
            public string oriFileName;
            public string changedFileName;

            public OriFileNameToChanged(string oriFileName, string changedFileName)
            {
                this.oriFileName = oriFileName;
                this.changedFileName = changedFileName;
            }
            public OriFileNameToChanged()
            {
                oriFileName = "";
                changedFileName = "";
            }
        }
        private List<OriFileNameToChanged> ChangedFiles { get; set; } = new List<OriFileNameToChanged>();
        private List<OriFileNameToChanged> ChangedFailedFiles { get; set; } = new List<OriFileNameToChanged>();

        BeingProcessed bp = new BeingProcessed();

        /// <summary>
        ///  폼 초기화
        /// </summary>
        public ChangeName()
        {
            //  폼 초기화(Designer.cs)
            InitializeComponent();

            //  configData 불러오기
            ConfigDataLoad();
            
            //  인스턴스 설정
            instance = this;
        }

        /// <summary>
        /// 폴더 선택 버튼 클릭 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFolder_Click(object sender, EventArgs e)
        {


            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "폴더를 선택해 주세요.";

            fbd.RootFolder = Environment.SpecialFolder.MyComputer;

            try
            {
                if (cbxFolder.Text != "") fbd.SelectedPath = cbxFolder.Text;
            }
            catch (Exception)
            {

            }

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                cbxFolder.Text = fbd.SelectedPath;
            }

        }

        /// <summary>
        /// 취소 버튼 클릭 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalcel_Click(object sender, EventArgs e)
        {
            //  어플리케이션 종료
            Application.Exit();
        }

        /// <summary>
        /// 확인 버튼 클릭 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {

            //  에러 메세지
            StringBuilder errMsg = new StringBuilder();

            //  입력 체크
            if (!ValidateNoInputCheck(ref errMsg))
            {
                //  입력 체크에 문제가 있으면 에러 메세지 출력
                MessageBox.Show(errMsg.ToString());
                //  처리 취소
                return;
            }

            //  폴더 검색 옵션
            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            if (ckbSubfolders.Checked) searchOption = SearchOption.AllDirectories;

            //  두번째 이후 변경시 복원 이름 초기화
            if( ChangedFiles != null) ChangedFiles.Clear();
            bp = null;
            bp = new BeingProcessed();

            //  이름 변경
            try
            {

                //  변경 갯수
                int count = 0;
                int failedCount = 0;

                string[] files;
                //  파일이름
                //  적용 확장자가 하나일 경우
                if (txtFileExt.Text.IndexOf(";") < 0)
                {
                    files = Directory.GetFiles(cbxFolder.Text, txtFileExt.Text, searchOption);

                }
                //  적용 확장자가 2 이상일 경우
                else
                {
                    var exts = txtFileExt.Text.Split(';');
                    files = exts.SelectMany(ext => Directory.GetFiles(cbxFolder.Text, ext, searchOption)).ToArray();
                }

                //  처리 윈도우 설정
                bp.Show();
                bp.SetMaxCount(files.Length);
                bp.SetValue(0);
                bp.SetCurrentFileCount("");
                bp.SetCurrentFileName("");

                //  이름별 처리
                for (int i = 0; i < files.Length; i++)
                {
                    //  파일명
                    string fileName = GetFileName(files[i], false).Trim();
                    //  확장자
                    string ext = GetExt(files[i], true);
                    //  경로
                    string path = GetPath(files[i], true);

                    //  처리 윈도우 표시
                    bp.SetCurrentFileName(path + fileName);
                    bp.SetCurrentFileCount(i.ToString() + "/" + files.Length.ToString());

                    //  확장자 변경
                    if (ckbChangeExt.Checked) ext = cbxChangeExt.Text.IndexOf(".") == 0 ? cbxChangeExt.Text : "." + cbxChangeExt.Text;


                    //  파일 이름 전부 바꿀 경우
                    if (ckbTextChange.Checked == true && cbxBeforeText.Text != "")
                    {
                        if (cbxBeforeText.Text == "*.*")
                        {
                            fileName = cbxAfterText.Text;
                        }
                        else
                        {
                            fileName = fileName.Replace(cbxBeforeText.Text, cbxAfterText.Text);
                        }
                    }

                    //구간 위치 변경
                    if (ckbPositionChange.Checked)
                    {
                        int positionChangeStartIndex = -1;
                        int positionChangeEndIndex = -1;
                        int positionChangeLength = -1;
                        if (string.IsNullOrEmpty(cbxPositionChangeStart.Text) != true
                            && string.IsNullOrEmpty(cbxPositionChangeEnd.Text) != true)
                        {
                            positionChangeStartIndex = GetPostionChangeIndex(fileName, cbxPositionChangeStart.Text);
                            positionChangeEndIndex = GetPostionChangeIndex(fileName, cbxPositionChangeEnd.Text, positionChangeStartIndex);
                            positionChangeEndIndex = cbxPositionChangeEnd.Text.Length == 1 ? positionChangeEndIndex : positionChangeEndIndex + cbxPositionChangeEnd.Text.Length - 1;
                            if (positionChangeStartIndex < 0 || positionChangeEndIndex < 0) positionChangeLength = 0;
                            else positionChangeLength = positionChangeEndIndex - positionChangeStartIndex + 1;
                        }
                        else if (string.IsNullOrEmpty(cbxPositionChangeStart.Text) != true)
                        {
                            positionChangeStartIndex = GetPostionChangeIndex(fileName, cbxPositionChangeStart.Text);
                            positionChangeEndIndex = positionChangeStartIndex + cbxPositionChangeStart.Text.Length - 1;
                            if (positionChangeStartIndex < 0 || positionChangeEndIndex < 0) positionChangeLength = 0;
                            else positionChangeLength = cbxPositionChangeStart.Text.Length;

                        }
                        else if (string.IsNullOrEmpty(cbxPositionChangeEnd.Text) != true)
                        {
                            positionChangeStartIndex = GetPostionChangeIndex(fileName, cbxPositionChangeEnd.Text);
                            positionChangeEndIndex = positionChangeStartIndex + cbxPositionChangeEnd.Text.Length - 1;
                            if (positionChangeStartIndex < 0 || positionChangeEndIndex < 0) positionChangeLength = 0;
                            else positionChangeLength = cbxPositionChangeEnd.Text.Length;

                        }

                        bool positionChangeUse = true;
                        if (ckbPositionChangeFirst.Checked == true && positionChangeStartIndex != 0) positionChangeUse = false;
                        if (ckbPositionChangeLast.Checked == true && positionChangeEndIndex != fileName.Length - 1) positionChangeUse = false;

                        if (positionChangeUse == true && positionChangeStartIndex >= 0 && positionChangeLength > 0) 
                        {
                            string positionChangeStr = fileName.Substring(positionChangeStartIndex, positionChangeLength);

                            string tempFileName = fileName.Substring(0, positionChangeStartIndex);
                            if (fileName.Length > positionChangeEndIndex) tempFileName += fileName.Substring(positionChangeEndIndex + 1);
                            fileName = tempFileName.Trim();



                            if (rdbPositionChangeFirst.Checked == true)
                            {
                                fileName = positionChangeStr + fileName;
                            }
                            else
                            {
                                fileName = fileName + positionChangeStr;
                            }

                        }
                    }

                    //  머릿말
                    if (ckbHead.Checked) fileName = cbxHead.Text + fileName;

                    //  꼬릿말
                    if (ckbTail.Checked)
                    {
                        fileName += cbxTail.Text;
                    }

                    //  카운트
                    if (ckbCount.Checked)
                    {
                        StringBuilder sb = new StringBuilder();

                        //  제일 앞부분에 추가
                        if (rdbHead.Checked)
                        {
                            sb.Append(cbxCountHead.Text);

                            //  카운트
                            StringBuilder num = new StringBuilder();
                            num.Append((i + 1).ToString());
                            int number = int.Parse(cbxCountNumber.Text) - num.Length;
                            if (number > 0)
                            {
                                for (int j = 0; j < number; j++)
                                {
                                    num.Insert(0, "0");
                                }
                            }
                            sb.Append(num.ToString());
                            sb.Append(cbxCountTail.Text);
                            sb.Append(fileName);

                        }
                        //  제일 뒷부분에 추가
                        else
                        {
                            sb.Append(fileName);
                            sb.Append(cbxCountHead.Text);

                            //  카운트
                            StringBuilder num = new StringBuilder();
                            num.Append((i + 1).ToString());
                            int number = int.Parse(cbxCountNumber.Text) - num.Length;
                            if (number > 0)
                            {
                                for (int j = 0; j < number; j++)
                                {
                                    num.Insert(0, "0");
                                }
                            }
                            sb.Append(num.ToString());

                            sb.Append(cbxCountTail.Text);
                        }

                        fileName = sb.ToString();

                    }

                    //  이름 변환
                    string changeFileName = path + fileName + ext;
                    if (files[i] != changeFileName)
                    {

                        changeFileName = SameNameToCountName(changeFileName);
                        try
                        {
                            File.Move(files[i], changeFileName);
                            OriFileNameToChanged ofc = new OriFileNameToChanged(files[i], changeFileName);
                            Log.Line($"[{files[i]}]을 [{changeFileName}]로 변경 했습니다.");
                            ChangedFiles.Add(ofc);
                            count++;
                        }
                        catch (Exception ex)
                        {
                            OriFileNameToChanged ofc = new OriFileNameToChanged(files[i], changeFileName);
                            ChangedFailedFiles.Add(ofc);
                            failedCount++;
                            Log.Error(ex);
#if DEBUG
                            MessageBox.Show(ex.ToString());
#endif
                        }
                    }

                    bp.SetValue(i + 1);

                }

                //  실행 시
                if (!isCancel)
                {
                    if (MessageBox.Show($"총 {count}개의 이름을 변경 하였습니다."
                        + $"총 {failedCount}개의 이름을 변경에 실패 하였습니다.") == DialogResult.OK)
                    {
                        //bp.Close();
                        //this.Enabled = true;
                    }
                    // ConfigData 저장하기
                    ConfigDataSave();
                }
                //  캔슬 시
                else
                {
                    ReChangeFileName(ChangedFiles.Count, ChangedFiles.Count);
                    if (MessageBox.Show("변경을 취소 하였습니다.") == DialogResult.OK)
                    {
                        isCancel = false;
                        bp.Close();
                        //this.Enabled = true;
                    }
                }


            }
            //  에러 시
            catch (Exception ee)
            {
                //  파일 이름 변경 복원
                ReChangeFileName(ChangedFiles.Count, ChangedFiles.Count);
                if (MessageBox.Show("이름 변경에 실패 했습니다.") == DialogResult.OK)
                {
                    bp.Close();
                    //this.Enabled = true;
                }
                Log.Error(ee);
#if DEBUG
                MessageBox.Show(ee.ToString());
#endif





            }

        }


        /// <summary>
        /// 변경된 이름 복원
        /// </summary>
        /// <param name="length">총 개수</param>
        /// <param name="changedFilesCount">변경된 이름의 개수</param>
        private void ReChangeFileName(int length, int changedFilesCount)
        {

            for (int i = 0; i < changedFilesCount; i++)
            {
                OriFileNameToChanged otc = ChangedFiles[changedFilesCount - i - 1];
                bp.SetCurrentFileName(otc.oriFileName);
                bp.SetCurrentFileCount((changedFilesCount - i).ToString() + "/" + length.ToString());

                File.Move(otc.changedFileName, otc.oriFileName);

                bp.SetValue(changedFilesCount - (i + 1));
            }



        }

        /// <summary>
        /// 변경된 이름 복원
        /// </summary>
        public void ReChangeFileName()
        {
            ReChangeFileName(ChangedFiles.Count, ChangedFiles.Count);
            if (MessageBox.Show("이름을 복원 했습니다.") == DialogResult.OK)
            {
                bp.Close();
            }
        }


        /// <summary>
        /// 콤보 박스에서 파일 형식을 선택할 경우 파일형식 TextBox란에 선택한 파일 형식을 격납
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxFileExt_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFileExt.Text = cbxFileExt.SelectedItem.ToString();
        }

        /// <summary>
        /// 머릿말 입력 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbHead_CheckedChanged(object sender, EventArgs e)
        {
            cbxHead.Enabled = ckbHead.Checked;
        }

        /// <summary>
        /// 꼬릿말 입력 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbTail_CheckedChanged(object sender, EventArgs e)
        {
            cbxTail.Enabled = ckbTail.Checked;
        }

        /// <summary>
        /// 카운트 사용 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbCount_CheckedChanged(object sender, EventArgs e)
        {
            cbxCountHead.Enabled = ckbCount.Checked;
            cbxCountTail.Enabled = ckbCount.Checked;
            cbxCountNumber.Enabled = ckbCount.Checked;
            gpbCount.Enabled = ckbCount.Checked;

        }

        /// <summary>
        /// 구간 위치 변경 사용 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbPositionChange_CheckedChanged(object sender, EventArgs e)
        {
            cbxPositionChangeStart.Enabled = ckbPositionChange.Checked;
            cbxPositionChangeEnd.Enabled = ckbPositionChange.Checked;
            gpbPositionChange.Enabled = ckbPositionChange.Checked;
            ckbPositionChangeFirst.Enabled = ckbPositionChange.Checked;
            ckbPositionChangeLast.Enabled = ckbPositionChange.Checked;

        }

        /// <summary>
        /// 구간 위치 변경 머리말 한정 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbPositionChangeFirst_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbPositionChangeFirst.Checked == true) ckbPositionChangeLast.Checked = false;
        }

        /// <summary>
        /// 구간 위치 변경 꼬리말 한정 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbPositionChangeLast_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbPositionChangeLast.Checked == true) ckbPositionChangeFirst.Checked = false;

        }


        /// <summary>
        /// 카운트 자릿수 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxCountNumber_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbxCountNumber.Text == "") return;

                int num = int.Parse(cbxCountNumber.Text);
                if (num < 0 || num > 99)
                {
                    MessageBox.Show("1~99 사이로 입력해 주세요.");
                    cbxCountNumber.Text = "2";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("1~99 사이로 입력해 주세요.");
                cbxCountNumber.Text = "2";

            }
        }

        /// <summary>
        /// 확장자 변경 적용 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbChangeExt_CheckedChanged(object sender, EventArgs e)
        {
            cbxChangeExt.Enabled = ckbChangeExt.Checked;
        }

        /// <summary>
        /// 문자 변경 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbTextChange_CheckedChanged(object sender, EventArgs e)
        {
            cbxBeforeText.Enabled = ckbTextChange.Checked;
            cbxAfterText.Enabled = ckbTextChange.Checked;

        }


        /// <summary>
        /// 콘피그 데이터 불러오기
        /// </summary>
        /// <returns></returns>
        private bool ConfigDataLoad()
        {
            try
            {
                FileStream loadFile = File.Open(CONFIG_NAME, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                ConfigData = (ConfigData)formatter.Deserialize(loadFile);

                loadFile.Close();

                GetConfigData();

                return true;
            }
            catch (Exception)
            {
                ConfigData = new ConfigData();
                return false;
            }
        }



        /// <summary>
        /// 콘피그 데이터 취득
        /// </summary>
        private void GetConfigData()
        {
            //  폳더 이력
            AddComboxData(ConfigData.lstFolder, cbxFolder);

            //  선택 파일 형식
            txtFileExt.Text = ConfigData.sFileExt;

            //  선택 파일 형식
            AddComboxData(ConfigData.lstFileExt, cbxFileExt);

            //  서브 폴더 검색 체크
            ckbSubfolders.Checked = ConfigData.isSubFolder;

            //  확장자 적용 체크
            ckbChangeExt.Checked = ConfigData.isChangeExt;
            //  확장자 변경 이력 
            AddComboxData(ConfigData.lstChangeExt, cbxChangeExt);

            //  머릿말 달기 체크
            ckbHead.Checked = ConfigData.isHead;
            //  머릿말 이력 
            AddComboxData(ConfigData.lstHead, cbxHead);

            //  꼬릿말 달기 체크
            ckbTail.Checked = ConfigData.isTail;
            //  꼬릿말 이력 
            AddComboxData(ConfigData.lstTail, cbxTail);

            //  카운트 체크
            ckbCount.Checked = ConfigData.isCount;
            //  카운트 위치 체크(머리)
            rdbHead.Checked = !ConfigData.isCountBack;
            //  카운트 위치 체크(꼬리)
            rdbTail.Checked = ConfigData.isCountBack;
            //  카운트 자릿수 이력
            AddComboxData(ConfigData.lstCountNumber, cbxCountNumber);
            //  카운트 머릿말 이력
            AddComboxData(ConfigData.lstCountHead, cbxCountHead);
            //  카운트 꼬릿말 이력
            AddComboxData(ConfigData.lstCountTail, cbxCountTail);

            //  구간 위치 변경 체크
            ckbPositionChange.Checked = ConfigData.isPositionChange;
            //  구간 위치 변경 위치 체크(머리)
            rdbPositionChangeFirst.Checked = !ConfigData.isPositionChangeBack;
            //  구간 위치 변경 위치 체크(꼬리)
            rdbPositionChangeLast.Checked = ConfigData.isPositionChangeBack;
            //  구간 위치 변경 단어시작 이력
            AddComboxData(ConfigData.lstPositionChangeStart, cbxPositionChangeStart);
            //  구간 위치 변경 단어끝 이력
            AddComboxData(ConfigData.lstPositionChangeEnd, cbxPositionChangeEnd);
            //  구간 위치 변경 머리말 한정 체크
            ckbPositionChangeFirst.Checked = ConfigData.isPositionChangeFirst;
            //  구간 위치 변경 꼬리말 한정 체크
            ckbPositionChangeLast.Checked = ConfigData.isPositionChangeLast;

            //  구간 위치 변경 체크
            ckbTextChange.Checked = ConfigData.isTextChange;
            //  변경전 문자 이력
            AddComboxData(ConfigData.lstBeforeText, cbxBeforeText);
            //  변경후 문자 이력
            AddComboxData(ConfigData.lstAfterText, cbxAfterText);
        }

        /// <summary>
        /// 콤보박스에 데이터 설정
        /// </summary>
        /// <param name="list">원본 리스트</param>
        /// <param name="comboBox">복사 콤보박스</param>
        private void AddComboxData(List<string> list, ComboBox comboBox)
        {
            comboBox.Items.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                comboBox.Items.Add(list[i]);
            }
            if (comboBox.Items.Count > 0) comboBox.Text = (string)comboBox.Items[0];
        }

        /// <summary>
        /// 콘피그 데이터 저장하기
        /// </summary>
        private void ConfigDataSave()
        {
            if (ConfigData == null) return;

            FileStream savefile = File.Create(CONFIG_NAME);
            BinaryFormatter formatter = new BinaryFormatter();

            SetConfigData();

            formatter.Serialize(savefile, ConfigData);
            savefile.Close();

        }


        /// <summary>
        /// 콘피그 데이터 저장
        /// </summary>
        private void SetConfigData()
        {
            ConfigData con = new ConfigData();

            //  폳더 이력
            AddListData(con.lstFolder, cbxFolder);

            //  선택 파일 형식
            con.sFileExt = txtFileExt.Text;

            //  선택 파일 형식
            AddListString(con.lstFileExt, cbxFileExt, txtFileExt.Text);

            //  서브 폴더 검색 체크
            con.isSubFolder = ckbSubfolders.Checked;

            //  확장자 적용 체크
            con.isChangeExt = ckbChangeExt.Checked;
            //  확장자 변경 이력 
            AddListData(con.lstChangeExt, cbxChangeExt);

            //  머릿말 달기 체크
            con.isHead = ckbHead.Checked;
            //  머릿말 이력 
            AddListData(con.lstHead, cbxHead);

            //  꼬릿말 달기 체크
            con.isTail = ckbTail.Checked;
            //  꼬릿말 이력 
            AddListData(con.lstTail, cbxTail);

            //  카운트 체크
            con.isCount = ckbCount.Checked;
            //  카운트 위치 체크(꼬리)
            con.isCountBack = rdbTail.Checked;
            //  카운트 자릿수 이력
            AddListData(con.lstCountNumber, cbxCountNumber);

            //  카운트 머릿말 이력
            AddListData(con.lstCountHead, cbxCountHead);

            //  카운트 꼬릿말 이력
            AddListData(con.lstCountTail, cbxCountTail);

            //  구간 위치 변경 체크
            con.isPositionChange = ckbPositionChange.Checked;
            //  구간 위치 변경 위치 체크
            con.isPositionChangeBack = rdbPositionChangeLast.Checked;
            //  구간 위치 변경 단어시작 이력
            AddListData(con.lstPositionChangeStart, cbxPositionChangeStart);
            //  구간 위치 변경 단어끝 이력
            AddListData(con.lstPositionChangeEnd, cbxPositionChangeEnd);
            //  구간 위치 변경 머리말 한정 체크
            con.isPositionChangeFirst = ckbPositionChangeFirst.Checked;
            //  구간 위치 변경 꼬리말 한정 체크
            con.isPositionChangeLast = ckbPositionChangeLast.Checked;

            //  구간 위치 변경 체크
            con.isTextChange = ckbTextChange.Checked;
            //  변경 전 문자 이력
            AddListData(con.lstBeforeText, cbxBeforeText);
            //  변경 후 문자 이력
            AddListData(con.lstAfterText, cbxAfterText);

            ConfigData = con;
        }

        /// <summary>
        /// 데이터 설정
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="comboBox">추가할 콤보박스데이터</param>
        private void AddListData(List<String> list, ComboBox comboBox)
        {
            string currentText = comboBox.Text;
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (currentText == (string)comboBox.Items[i])
                {
                    comboBox.Items.Remove(currentText);
                    break;
                }
            }
            comboBox.Items.Insert(0, currentText);
            comboBox.Text = currentText;
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                list.Add((string)comboBox.Items[i]);
            }
        }

        /// <summary>
        /// 리스트에 문자열을 추가
        /// </summary>
        /// <param name="list"></param>
        /// <param name="comboBox"></param>
        private void AddListString(List<String> list, ComboBox comboBox, string text)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (text == (string)comboBox.Items[i])
                {
                    comboBox.Items.Remove(text);
                    break;
                }
            }
            comboBox.Items.Insert(0, text);
            comboBox.Text = text;

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                list.Add((string)comboBox.Items[i]);
            }
        }

        /// <summary>
        /// 체크 데이터 설정
        /// </summary>
        /// <param name="isbool">설정할 bool</param>
        /// <param name="checkBox">대입할 체크박스</param>
        private void AddBoolData(ref bool isbool, CheckBox checkBox)
        {
            isbool = checkBox.Checked;
        }

        /// <summary>
        /// 입력 내용 체크
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private bool ValidateNoInputCheck(ref StringBuilder errMsg)
        {

            //  폴더 체크
            if (cbxFolder.Text == "") errMsg.Append("폴더를 입력해 주세여." +"\r\n");
            
            //  파일 형식 입력 체크
            if (txtFileExt.Text == "") errMsg.Append("파일 형식을 입력해 주세요." + "\r\n");

            //  확장자 체크

            //  머릿말 체크
            if (ckbHead.Checked)
            {
                if (cbxHead.Text == "") errMsg.Append("머릿말을 입력해 주세요." + "\r\n");
            }
            
            //  꼬릿말 체크
            if (ckbTail.Checked)
            {
                if (cbxTail.Text == "") errMsg.Append("꼬릿말을 입력해 주세요." + "\r\n");
            }

            //  카운터 체크
            if (ckbCount.Checked)
            {
                if(cbxCountNumber.Text == "") errMsg.Append("카운트 자릿수를 입력해 주세요." + "\r\n");
            }

            //  구간 위치 변경 체크
            if (ckbPositionChange.Checked)
            {
                if (string.IsNullOrEmpty(cbxPositionChangeStart.Text) && string.IsNullOrEmpty(cbxPositionChangeEnd.Text)) errMsg.Append("구간 위치 변경 문자를 입력해 주세요." + "\r\n");
            }

            //  변경전 문자 체크
            if (cbxBeforeText.Text == "")
            {
                if( !ckbCount.Checked && !ckbHead.Checked && !ckbTail.Checked && !ckbPositionChange.Checked) errMsg.Append("바꿀 문자를 입력해 주세요." + "\r\n");
            }
            else if (cbxBeforeText.Text == "*.*")
            {
                if (!ckbCount.Checked) errMsg.Append("카운트할 방식을 입력해 주세요." + "\r\n");
            }


            if (errMsg.Length != 0) return false;

            return true;
        }


        /// <summary>
        /// 같은 이름이 있을경우 카운트 붙임
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private string SameNameToCountName(string fullPath)
        {
            string rtn = fullPath;
            if( File.Exists(fullPath) == true)
            {
                int i = 0;
                string filename = GetFileName(fullPath, false);
                string ext = GetExt(fullPath, true);
                string path = GetPath(fullPath, true);
                string nextFilename = "";
                while (true)
                {
                    i++;
                    string tempFilename = filename + $" ({i})";
                    nextFilename = path + tempFilename + ext;
                    if (File.Exists(nextFilename) == false) break;
                }
                rtn = nextFilename;
            }


            return rtn;

        }


        /// <summary>
        /// 파일명 취득
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="isExt">확장자 포함 </param>
        /// <returns></returns>
        private string GetFileName(string fullPath, bool isExt = true)
        {
            string rtn = "";

            string fileName = Path.GetFileName(fullPath);
            
            //  파일명
            rtn = isExt ? fileName : fileName.Substring(0, fileName.LastIndexOf("."));

            return rtn;
        }

        /// <summary>
        /// 파일명 취득
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="isAddDot">확장자 앞에 .을 붙이는지 확인</param>
        /// <returns></returns>
        private string GetExt(string fullPath, bool isAddDot = false)
        {
            string rtn = "";

            string fileName = Path.GetFileName(fullPath);

            //  확장자
            rtn = isAddDot ? fileName.Substring(fileName.LastIndexOf(".")) : fileName.Substring(fileName.LastIndexOf(".") + 1);


            return rtn;
        }


        /// <summary>
        /// 경로 취득
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="isAddSlash">경로뒤에\추가확인</param>
        /// <returns></returns>
        private string GetPath(string fullPath, bool isAddSlash = true)
        {
            string rtn = "";
            //  경로
            string path = Path.GetDirectoryName(fullPath);

            rtn = isAddSlash ? path + "\\" : path;

            return rtn;
        }


        /// <summary>
        /// 구간 변경 위치 취득
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="positionChangeStr"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private int GetPostionChangeIndex(string filename, string positionChangeStr, int startIndex = -1)
        {
            int rtn = -1;
            if (filename == null || positionChangeStr == null || positionChangeStr.Trim() == "") return rtn;
            if (startIndex >= filename.Length) startIndex = filename.Length - 1;
            rtn = startIndex < 0 ? filename.IndexOf(positionChangeStr) : filename.IndexOf(positionChangeStr, startIndex);
            return rtn;
        }


    }
}
