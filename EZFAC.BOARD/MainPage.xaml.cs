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
        string content;


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
            getTimer.Interval = new TimeSpan(0, 0, 3);
            getTimer.Tick += GetTimer_Tick;
          //  getTimer.Start();

            // 加载异常信息列表
            lvFiles.Items.Add(new
            {
                  GW = "压轴线A-01",
                  WT = "停机",
                  FSSJ = "2018-06-24",
                  ZT = "关机"
            });
            getUserList();
           // getContent();
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            timetag.Text = DateTime.Now.ToString();
        }

        private async void GetTimer_Tick(object sender, object e)
        {
            content = null;
            getContent();
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
                    string GW = null, WT = null, FSSJ = null, ZT = null;
                    foreach (string rowDetail in rowDetails)
                    {
                        keyAndValue = rowDetail.Split(':');

                        if ("user_name".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "user_name")
                        {
                            GW = keyAndValue[1].Trim();
                        }
                        else if ("user_password".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "user_password")
                        {
                            WT = keyAndValue[1].Trim();
                        }
                        else if ("level".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "level")
                        {
                            FSSJ = keyAndValue[1].Trim();
                        }
                        else if ("authority".Equals(keyAndValue[0].Trim()) || keyAndValue[0].Trim() == "authority")
                        {
                            ZT = keyAndValue[1].Trim();
                        }
                    }
                    lvFiles.Items.Add(new
                    {
                        GW = GW,
                        WT = WT,
                        FSSJ = FSSJ,
                        ZT = ZT
                    });
                }
            }

        }

        private async void getContent()
        {
            ContentDialog dialog = new ContentDialog()
            {
                FontSize = 32d,
                Content = "ss",
                PrimaryButtonText = "确定",
            };
            
            string url = httpUrl + "/get-userInfo?uuid=" + random.Next();
            var httpClient = new HttpClient();
            var resourceUri = new Uri(url);
            HttpResponseMessage response = null;
            string msg = "ss",content = null;
            try
            {
                response = await httpClient.GetAsync(resourceUri);
                content = response.Content.ToString();
                dialog.Content = content;
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            finally
            {
                await dialog.ShowAsync();
                if (response != null)
                {
                    response.Dispose();
                }
                httpClient.Dispose();
            }
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
                response = await httpClient.GetAsync(resourceUri);
                content = response.Content.ToString();
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
