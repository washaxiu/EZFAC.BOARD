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
        private SolidColorBrush red = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        private SolidColorBrush textRed = new SolidColorBrush(Color.FromArgb(255, 255, 204, 204));
        private SolidColorBrush gray = new SolidColorBrush(Color.FromArgb(255, 102, 102, 102));
        private SolidColorBrush textGray = new SolidColorBrush(Color.FromArgb(255, 188, 188, 188));
        private SolidColorBrush green = new SolidColorBrush(Color.FromArgb(255, 0, 204, 0));
        private SolidColorBrush textGreen = new SolidColorBrush(Color.FromArgb(255, 204, 255, 102));

        public string httpUrl = "http://192.168.1.110:8800";

        public string[] deviceName = { "LXJ0001", "LXJ0002", "LXJ0003","JCJ0001", "JCJ0002", "YYJ0001", "YYJ0002", "LDJ0001", "LDJ0002", "LDJ0003",
                                       "JXS0001", "JXS0002", "DQFS001", "YWGZ001", "YWGZ002", "HLJ0001", "HLJ0002", "HLJ0003",
                                       "RJL0001", "RJL0002", "RJL0003", "RJL0004", "YZA0001", "YZA0002", "YZA0003", "YZA0004", "YZA0005", "YZA0006",
                                       "YZA0007", "YZA0008", "YZA0009", "YZA0012", "YZA0013", "YZA0014", "YZA0015", "YZA0016", "YZA0017", "YZA0018",
                                       "YZA0019", "YZB0001", "YZB0002", "YZB0003", "YZB0004", "YZB0005", "YZB0006", "YZB0007", "YZB0008", "YZB0009",
                                       "YZB0012", "YZB0013", "YZB0014", "YZB0015", "YZB0016", "YZB0017", "YZB0018", "YZB0019", "YZC0001", "YZC0002",
                                       "YZC0003", "YZC0004", "YZC0005", "YZC0006", "YZD0001", "YZD0002", "YZD0003", "YZD0004", "YZD0005", "YZD0006"};

        public string[] deviceCnm = {"离型剂供给机-01","离型剂供给机-02","离型剂供给机-03", "集尘机-01", "集尘机-02", "油烟机-01", "油烟机-02", "冷冻机-01",
                                       "冷冻机-02", "冷冻机-03","机械手信号-01", "机械手信号-02" ,"氮气发生装置-01","液位感知-01", "液位感知-02",
                                       "回料机-01", "回料机-02", "回料机-03" ,"溶解炉-01", "溶解炉-02", "溶解炉-03", "溶解炉-04" ,
                                       "压铸线-A01", "压铸线-A02", "压铸线-A03", "压铸线-A04", "压铸线-A05", "压铸线-A06", "压铸线-A07", "压铸线-A08",
                                       "压铸线-A09", "压铸线-A12", "压铸线-A13", "压铸线-A14", "压铸线-A15", "压铸线-A16", "压铸线-A17", "压铸线-A18",
                                       "压铸线-A19","压铸线-B01", "压铸线-B02", "压铸线-B03", "压铸线-B04", "压铸线-B05", "压铸线-B06", "压铸线-B07",
                                       "压铸线-B08", "压铸线-B09", "压铸线-B12" ,"压铸线-B13", "压铸线-B14", "压铸线-B15", "压铸线-B16", "压铸线-B17",
                                       "压铸线-B18", "压铸线-B19","压铸线-C01", "压铸线-C02", "压铸线-C03", "压铸线-C04", "压铸线-C05", "压铸线-C06",
                                       "压铸线-D01", "压铸线-D02", "压铸线-D03", "压铸线-D04", "压铸线-D05", "压铸线-D06" };
        Random random = new Random();
        private int YZgreenCount = 0, YZredCount = 0, YZgrayCount = 0;
        private int FZgreenCount = 0, FZredCount = 0, FZgrayCount = 0;
        private int YZtotal = 46, FZtotal = 22, quantityToal = 0;
        private string message = null;

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
      
          /*  // 加载异常信息列表
            for (int i = 0; i < 3; i++)
            {
                lvFiles.Items.Add(new
                {
                    GW = "压铸线A-01"+i,
                    WT = "设备停机",
                    FSSJ = "2018-06-24"
                });
            }
             //   getUserList();
              //  getContent();*/
            // getYZInfo();
        }

        private async void getYZInfo()
        {

            ContentDialog dialog = new ContentDialog()
            {
                FontSize = 32d,
                Content = B08.Background.ToString(),
                PrimaryButtonText = "确定",
            };
            await dialog.ShowAsync();
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
                    lvFiles.Items.Clear();
                    YZredCount = 0; YZgrayCount = 0;
                    FZredCount = 0; FZgrayCount = 0;
                    quantityToal = 0;
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
                                if (!dic.ContainsKey(equipmentId))
                                {
                                    name = null;
                                    break;
                                }
                                name = dic[equipmentId];
                            }
                            else if ("current_status".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "current_status")
                            {
                                currentStatus = keyAndValue[1].Trim();
                            }
                            else if ("product_quantity".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "product_quantity")
                            {
                                produceQuantity = keyAndValue[1].Trim();
                                int quantity = Convert.ToInt32(produceQuantity);
                                quantityToal += quantity;
                                produceQuantity = quantity.ToString("0000");
                            }
                            else if ("recordtime".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "recordtime")
                            {
                                recordTime = keyAndValue[1].Trim()+":"+ keyAndValue[2].Trim()+ ":" + keyAndValue[3].Trim();
                            }
                        }
                        // 获取的信息回填
                        if (name != null)
                        {
                            setDataInfo(name, currentStatus, produceQuantity, recordTime);
                        }
                    }
                    YZgreenCount = YZtotal - YZgrayCount - YZredCount;
                    FZgreenCount = FZtotal - FZgrayCount - FZredCount;
                    YZgreenTotal.Content = YZgreenCount.ToString() + "台";
                    YZredTotal.Content = YZredCount.ToString() + "台";
                    YZgrayTotal.Content = YZgrayCount.ToString() + "台";
                    FZgreenTotal.Content = FZgreenCount.ToString() + "台";
                    FZredTotal.Content = FZredCount.ToString() + "台";
                    FZgrayTotal.Content = FZgrayCount.ToString() + "台";
                    total.Content = quantityToal;
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
            Button[] A = { A01, A02, A03, A04, A05, A06, A07, A08,
                             A09, A12, A13, A14, A15, A16, A17, A18,
                             A19};
            Button[] B = { B01, B02, B03, B04, B05, B06, B07, B08,
                             B09, B12, B13, B14, B15, B16, B17, B18,
                             B19};
            Button[] C = { C01, C02, C03, C04, C05, C06};
            Button[] D = { D01, D02, D03, D04, D05, D06};

            Button[] AA = { A_01, A_02, A_03, A_04, A_05, A_06, A_07, A_08,
                               A_09, A_12, A_13, A_14, A_15, A_16, A_17, A_18,
                               A_19};
            Button[] BB = { B_01, B_02, B_03, B_04, B_05, B_06, B_07, B_08,
                               B_09, B_12, B_13, B_14, B_15, B_16, B_17, B_18,
                               B_19};
            Button[] CC = { C_01, C_02, C_03, C_04, C_05, C_06};
            Button[] DD = { D_01, D_02, D_03, D_04, D_05, D_06};

            SolidColorBrush color = null,textColor = null;
            if (currentStatus == "r" || currentStatus == "R")
            {
                color = red;
                textColor = textRed;
            }
            else if (currentStatus == "o" || currentStatus == "O")
            {
                color = gray;
                textColor = textGray;
            }
            else
            {
                color = green;
                textColor = textGreen;
            }
            string[] nameInfo = name.Split('-');
            int num = 0;
            if (nameInfo[0] == "压铸线")
            {
                message = name;
                if (currentStatus == "r" || currentStatus == "R")
                {
                    YZredCount++;
                }
                else if (currentStatus == "o" || currentStatus == "O")
                {
                    YZgrayCount++;
                }
                num = getNum(nameInfo[1].Substring(1));
                if (nameInfo[1][0] == 'A')
                {
                    if(num<=9)
                    {
                        A[num - 1].Background = color;
                        AA[num - 1].Background = textColor;
                        AA[num - 1].Content = produceQuantity;
                    }
                    else
                    {
                        A[num - 3].Background = color;
                        AA[num - 3].Background = textColor;
                        AA[num - 3].Content = produceQuantity;
                    }
                }
                else if (nameInfo[1][0] == 'B')
                {
                    if (num <= 9)
                    {
                        B[num - 1].Background = color;
                        BB[num - 1].Background = textColor;
                        BB[num - 1].Content = produceQuantity;
                    }
                    else
                    {
                        B[num - 3].Background = color;
                        BB[num - 3].Background = textColor;
                        BB[num - 3].Content = produceQuantity;
                    }
                }
                else if (nameInfo[1][0] == 'C')
                {
                    C[num - 1].Background = color;
                    CC[num - 1].Background = textColor;
                    CC[num - 1].Content = produceQuantity;
                }
                else if (nameInfo[1][0] == 'D')
                {
                    D[num - 1].Background = color;
                    DD[num - 1].Background = textColor;
                    DD[num - 1].Content = produceQuantity;
                }
            }
            else
            {
                if (currentStatus == "r" || currentStatus == "R")
                {
                    FZredCount++;
                }
                else if (currentStatus == "o" || currentStatus == "O")
                {
                    FZgrayCount++;
                }
                num = getNum(nameInfo[1]);
                if (nameInfo[0] == "离型剂供给机")
                {
                    if (num == 1)
                    {
                        message = M01.Content.ToString();
                        M01.Background = color;
                    }
                    else if (num == 2)
                    {
                        message = M02.Content.ToString();
                        M02.Background = color;
                    }
                    else
                    {
                        message = M03.Content.ToString();
                        M03.Background = color;
                    }
                }
                else if (nameInfo[0] == "集尘机")
                {
                    if (num == 1)
                    {
                        message = L01.Content.ToString();
                        L01.Background = color;
                    }
                    else
                    {
                        message = L02.Content.ToString();
                        L02.Background = color;
                    }
                }
                else if (nameInfo[0] == "油烟机")
                {
                    if (num == 1)
                    {
                        message = K01.Content.ToString();
                        K01.Background = color;
                    }
                    else
                    {
                        message = K02.Content.ToString();
                        K02.Background = color;
                    }
                }
                else if (nameInfo[0] == "冷冻机")
                {
                    if (num == 1)
                    {
                        message = J01.Content.ToString();
                        J01.Background = color;
                    }
                    else if (num == 2)
                    {
                        message = J02.Content.ToString();
                        J02.Background = color;
                    }
                    else
                    {
                        message = J03.Content.ToString();
                        J03.Background = color;
                    }
                }
                else if (nameInfo[0] == "机械手信号")
                {
                    if (num == 1)
                    {
                        message = I01.Content.ToString();
                        I01.Background = color;
                    }
                    else
                    {
                        message = I02.Content.ToString();
                        I02.Background = color;
                    }
                }
                else if (nameInfo[0] == "氮气发生装置")
                {
                    message = H01.Content.ToString();
                    H01.Background = color;
                }
                else if (nameInfo[0] == "液位感知")
                {
                    if (num == 1)
                    {
                        message = G01.Content.ToString();
                        G01.Background = color;
                    }
                    else
                    {
                        message = G02.Content.ToString();
                        G02.Background = color;
                    }
                }
                else if (nameInfo[0] == "回料机")
                {
                    if (num == 1)
                    {
                        message = F01.Content.ToString();
                        F01.Background = color;
                    }
                    else if (num == 2)
                    {
                        message = F02.Content.ToString();
                        F02.Background = color;
                    }
                    else
                    {
                        message = F03.Content.ToString();
                        F03.Background = color;
                    }
                }
                else if (nameInfo[0] == "溶解炉")
                {
                    if (num == 1)
                    {
                        message = E01.Content.ToString();
                        E01.Background = color;
                    }
                    else if (num == 2)
                    {
                        message = E02.Content.ToString();
                        E02.Background = color;
                    }
                    else if (num == 3)
                    {
                        message = E03.Content.ToString();
                        E03.Background = color;
                    }
                    else
                    {
                        message = E04.Content.ToString();
                        E04.Background = color;
                    }
                }
            }
            if (currentStatus == "r" || currentStatus == "R")
            {
                lvFiles.Items.Add(new
                {
                    GW = message,
                    WT = "设备告警",
                    FSSJ = recordTime
                });
            }else if (currentStatus == "o" || currentStatus == "O")
            {
                lvFiles.Items.Add(new
                {
                    GW = message,
                    WT = "设备停机",
                    FSSJ = recordTime
                });
            } 
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
