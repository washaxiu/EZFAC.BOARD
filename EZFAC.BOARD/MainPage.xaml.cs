using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace EZFAC.BOARD
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer dispatcherTimer;
        DispatcherTimer getTimer;
        Dictionary<string, string> dic;
        private SolidColorBrush red = new SolidColorBrush(Colors.Red);
        private SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
        private SolidColorBrush green = new SolidColorBrush(Colors.Green);
        private SolidColorBrush yellow = new SolidColorBrush(Colors.Yellow);
        public string httpUrl = "http://192.168.2.110:8800";

        public string[] deviceName = { "LXJ0001", "JCJ0001", "JCJ0002", "YYJ0001", "YYJ0002", "LDJ0001", "LDJ0002", "LDJ0003",
                                       "JXS0001", "JXS0002", "DQFS001", "YWGZ001", "YWGZ002", "YWGZ003", "YWGZ004", "HLJ0001", "HLJ0002", "HLJ0003",
                                       "RJL0001", "RJL0002", "RJL0003", "RJL0004", "YZA0001", "YZA0002", "YZA0003", "YZA0004", "YZA0005", "YZA0006",
                                       "YZA0007", "YZA0008", "YZA0009", "YZA0012", "YZA0013", "YZA0014", "YZA0015", "YZA0016", "YZA0017", "YZA0018",
                                       "YZA0019", "YZB0001", "YZB0002", "YZB0003", "YZB0004", "YZB0005", "YZB0006", "YZB0007", "YZB0008", "YZB0009",
                                       "YZB0012", "YZB0013", "YZB0014", "YZB0015", "YZB0016", "YZB0017", "YZB0018", "YZB0019", "YZC0001", "YZC0002",
                                       "YZC0003", "YZC0004", "YZC0005", "YZC0006", "YZD0001", "YZD0002", "YZD0003", "YZD0004", "YZD0005", "YZD0006"};

        public string[] deviceCnm = {"离型剂供给机-01", "集尘机-01", "集尘机-02", "油烟机-01", "油烟机-02", "冷冻机-01",
                                       "冷冻机-02", "冷冻机-03","机械手信号-01", "机械手信号-02" ,"氮气发生装置-01","液位感知-01", "液位感知-02",
                                       "液位感知-03", "液位感知-04" ,"回料机-01", "回料机-02", "回料机-03" ,"溶解炉-01", "溶解炉-02", "溶解炉-03", "溶解炉-04" ,
                                       "压轴线-A01", "压轴线-A02", "压轴线-A03", "压轴线-A04", "压轴线-A05", "压轴线-A06", "压轴线-A07", "压轴线-A08",
                                       "压轴线-A09", "压轴线-A12", "压轴线-A13", "压轴线-A14", "压轴线-A15", "压轴线-A16", "压轴线-A17", "压轴线-A18",
                                       "压轴线-A19","压轴线-B01", "压轴线-B02", "压轴线-B03", "压轴线-B04", "压轴线-B05", "压轴线-B06", "压轴线-B07",
                                       "压轴线-B08", "压轴线-B09", "压轴线-B12" ,"压轴线-B13", "压轴线-B14", "压轴线-B15", "压轴线-B16", "压轴线-B17",
                                       "压轴线-B18", "压轴线-B19","压轴线-C01", "压轴线-C02", "压轴线-C03", "压轴线-C04", "压轴线-C05", "压轴线-C06",
                                       "压轴线-D01", "压轴线-D02", "压轴线-D03", "压轴线-D04", "压轴线-D05", "压轴线-D06" };
        Random random = new Random();

        public MainPage()
        {

            this.InitializeComponent();

            // 初始化 设备类型
            dic = new Dictionary<string, string>();
            for(int i=0;i< deviceName.Length; i++)
            {
                dic.Add(deviceName[i], deviceCnm[i]);
            }
            // 时间 定时器
            timetag.Text = DateTime.Now.ToString();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();

            // 数据获取 定时器
            getTimer = new DispatcherTimer();
            getTimer.Interval = new TimeSpan(0, 0, 10);
            getTimer.Tick += GetTimer_Tick;
            getTimer.Start();

            // 加载异常信息列表
            for (int i = 0; i < 3; i++)
            {
                lvFiles.Items.Add(new
                {
                    GW = "压轴线A-01"+i,
                    WT = "停机",
                    FSSJ = "2018-06-24",
                    ZT = "关机"
                });
            }
         //   getUserList();
          //  getContent();
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            timetag.Text = DateTime.Now.ToString();
        }

        private void GetTimer_Tick(object sender, object e)
        {
           // content = null;
            getContent();
        }

        private async void getContent()
        {
            ContentDialog dialog = new ContentDialog()
            {
                FontSize = 32d,
                Content = "",
                PrimaryButtonText = "确定",
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch
            };
            
            lvFiles.Items.Clear();
            string url = httpUrl + "/get-deviceConfig?uuid=" + random.Next();
            var httpClient = new HttpClient();
            var resourceUri = new Uri(url);
            HttpResponseMessage response = null;
            string msg = "ss",content = null;
            try
            {
                content = await httpClient.GetStringAsync(resourceUri);
                
                lvFiles.Items.Clear();
                if (content != null)
                {
                    
                    string[] detail = content.Split('}'), rowDetails = null, keyAndValue = null;
                    string rowContent = null;
                    int index = 0;
                    
                    for (int i = 0; i < detail.Length - 1; i++)
                    {
                        // 找到数据开始的位置
                        index = detail[i].IndexOf('{');

                        rowContent = detail[i].Substring(index + 1).Replace("\"", "");
                        rowDetails = rowContent.Split(',');
                       
                        string equipmentId = null, currentStatus = null, produceQuantity = null, recordTime = null,name = null;
                        foreach (string rowDetail in rowDetails)
                        {
                            keyAndValue = rowDetail.Split(':');

                            if ("equipment_id".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "equipment_id")
                            {
                                equipmentId = keyAndValue[1].Trim();
                                name = dic[equipmentId];
                            }
                            else if ("current_status".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "current_status")
                            {
                                currentStatus = keyAndValue[1].Trim();
                            }
                            else if ("product_quantity".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "product_quantity")
                            {
                                produceQuantity = keyAndValue[1].Trim();
                            }
                            else if ("recordtime".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "recordtime")
                            {
                                recordTime = keyAndValue[1].Trim();
                            }
                        }
                        // 获取的信息回填
                        setDataInfo(name, currentStatus, produceQuantity, recordTime);
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                }
                httpClient.Dispose();
            }
        }

        private void setDataInfo(string name, string currentStatus, string produceQuantity, string recordTime)
        {
            TextBox[] A = { A01, A02, A03, A04, A05, A06, A07, A08,
                             A09, A12, A13, A14, A15, A16, A17, A18,
                             A19};
            TextBox[] B = { B01, B02, B03, B04, B05, B06, B07, B08,
                             B09, B12, B13, B14, B15, B16, B17, B18,
                             B19};
            TextBox[] C = { C01, C02, C03, C04, C05, C06};
            TextBox[] D = { D01, D02, D03, D04, D05, D06};

            TextBlock[] AA = { A_01, A_02, A_03, A_04, A_05, A_06, A_07, A_08,
                               A_09, A_12, A_13, A_14, A_15, A_16, A_17, A_18,
                               A_19};
            TextBlock[] BB = { B_01, B_02, B_03, B_04, B_05, B_06, B_07, B_08,
                               B_09, B_12, B_13, B_14, B_15, B_16, B_17, B_18,
                               B_19};
            TextBlock[] CC = { C_01, C_02, C_03, C_04, C_05, C_06};
            TextBlock[] DD = { D_01, D_02, D_03, D_04, D_05, D_06};

            SolidColorBrush color = null;
            if (currentStatus == "r")
            {
                color = red;
            }else if(currentStatus == "y")
            {
                color = yellow;
            }
            else if (currentStatus == "g")
            {
                color = green;
            }
            else if (currentStatus == "o")
            {
                color = gray;
            }
            string GW = null, WT = null, FSSJ = null, ZT = null;
            string[] nameInfo = name.Split('-');
            int num = 0;
            if (nameInfo[0] == "压轴线")
            {
                num = getNum(nameInfo[1].Substring(1));
                if (nameInfo[1][0] == 'A')
                {
                    if(num<=9)
                    {
                        A[num - 1].Background = color;
                        AA[num - 1].Text = produceQuantity;
                    }
                    else
                    {
                        A[num - 3].Background = color;
                        AA[num - 3].Text = produceQuantity;
                    }
                }
                else if (nameInfo[1][0] == 'B')
                {
                    if (num <= 9)
                    {
                        B[num - 1].Background = color;
                        BB[num - 1].Text = produceQuantity;
                    }
                    else
                    {
                        B[num - 3].Background = color;
                        BB[num - 3].Text = produceQuantity;
                    }
                }
                else if (nameInfo[1][0] == 'C')
                {
                    C[num - 1].Background = color;
                    CC[num - 1].Text = produceQuantity;
                }
                else if (nameInfo[1][0] == 'D')
                {
                    D[num - 1].Background = color;
                    DD[num - 1].Text = produceQuantity;
                }
            }
            else
            {
                num = getNum(nameInfo[1]);
                if (nameInfo[0] == "离型剂供给机")
                {
                    M01.Background = color;
                }
                else if (nameInfo[0] == "集尘机")
                {
                    if(num==1)
                    {
                        L01.Background = color;
                    }else
                    {
                        L02.Background = color;
                    }
                }
                else if (nameInfo[0] == "油烟机")
                {
                    if (num == 1)
                    {
                        K01.Background = color;
                    }else
                    {
                        K02.Foreground = color;
                    }
                }
                else if (nameInfo[0] == "冷冻机")
                {
                    if (num == 1)
                    {
                        J01.Background = color;
                    }else if (num == 2)
                    {
                        J02.Background = color;
                    }else
                    {
                        J03.Background = color;
                    }
                }
                else if (nameInfo[0] == "机械手信号")
                {
                    if (num == 1)
                    {
                        I01.Background = color;
                    }else
                    {
                        I02.Background = color;
                    }
                }
                else if (nameInfo[0] == "氮气发生装置")
                {
                    H01.Background = color;
                }
                else if (nameInfo[0] == "液位感知")
                {
                    if (num == 1)
                    {
                        G01.Background = color;
                    }
                    else if (num == 2)
                    {
                        G02.Background = color;
                    }
                    else if (num == 3)
                    {
                        G03.Background = color;
                    }
                    else
                    {
                        G04.Background = color;
                    }
                }
                else if (nameInfo[0] == "回料机")
                {
                    if (num == 1)
                    {
                        F01.Background = color;
                    }else
                    {
                        F02.Background = color;
                    }
                }
                else if (nameInfo[0] == "溶解炉")
                {
                    if (num == 1)
                    {
                        E01.Background = color;
                    }
                    else if (num == 2)
                    {
                        E02.Background = color;
                    }
                    else if (num == 3)
                    {
                        E03.Background = color;
                    }
                    else
                    {
                        E04.Background = color;
                    }
                }
            }


            if (currentStatus == "r")
            {
                GW = name;
                WT = "错误";
                FSSJ = recordTime;
                ZT = "错误";
            }else if (currentStatus == "o")
            {
                GW = name;
                WT = "停机";
                FSSJ = recordTime;
                ZT = "停机";
            }

            lvFiles.Items.Add(new
            {
                GW = GW,
                WT = WT,
                FSSJ = FSSJ,
                ZT = ZT
            });
        }

        private int getNum(string str)
        {
            int num = 0;
            for(int i = 0; i < str.Length; i++)
            {
                num = num * 10 + str[i] - '0';
            }
            return num;
        }

        public async void getUserList()
        {
            ContentDialog dialog = new ContentDialog()
            {
                FontSize = 32d,
                Content = "ss",
                PrimaryButtonText = "确定",
            };

            string url = httpUrl + "/get-userInfo?uuid=" + random.Next();
            JsonObject checkRecordData = new JsonObject();
            string fileName = "user.json";
            var httpClient = new HttpClient();
            var resourceUri = new Uri(url);
            HttpResponseMessage response = null;
            string msg = null, content = null;
            try
            {
                content = await httpClient.GetStringAsync(resourceUri);
               // content = response.Content.ToString();
                dialog.Content = content;
                if (content != null)
                {

                    checkRecordData.Add("Users", JsonValue.CreateStringValue(content));
                }
            }
            catch (Exception e)
            {
                fileName = e.ToString();
            }
            finally
            {
                await dialog.ShowAsync();
                content = null;
                if (response != null)
                {
                    response.Dispose();
                }
                httpClient.Dispose();
            }
        }
    }
}
