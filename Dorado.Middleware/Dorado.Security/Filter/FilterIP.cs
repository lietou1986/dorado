using System.Web;

namespace Dorado.Security.Filter
{
    public class FilterIP
    {
        public static void CheckIp(string str)
        {
            string Ip = HttpContext.Current.Request.UserHostAddress;
            int Flag = 0;
            int i = 0;
            if (Ip != "")
            {
                for (int j = 1; j < 10; j++)
                {
                    string ApplicationItem = "checkip" + str + j.ToString();
                    if (HttpContext.Current.Application[ApplicationItem] != null)
                    {
                        string ApplicationContent = HttpContext.Current.Application[ApplicationItem].ToString();
                        if (ApplicationContent.IndexOf(Ip) > -1)
                        {
                            int IpLength = Ip.Length;
                            string TempStr = ApplicationContent.Substring(ApplicationContent.IndexOf(Ip));
                            int TimeLength = TempStr.IndexOf(")") - TempStr.IndexOf("(") - 1;
                            string OldTime = TempStr.Substring(TempStr.IndexOf("(") + 1, TimeLength);
                            System.TimeSpan timespan = System.Convert.ToDateTime(OldTime) - System.DateTime.Now;
                            if (timespan.Seconds > 20 || timespan.Seconds < 0)
                            {
                                HttpContext.Current.Application.Lock();
                                string tmps = ApplicationContent.Substring(0, ApplicationContent.IndexOf(Ip) + IpLength);
                                HttpContext.Current.Application[ApplicationItem] = tmps;
                                tmps = "(" + System.DateTime.Now.ToLongTimeString() + ApplicationContent.Substring(ApplicationContent.IndexOf(Ip) + IpLength + TimeLength + 1);
                                HttpApplicationState application;
                                string name;
                                (application = HttpContext.Current.Application)[name = ApplicationItem] = application[name] + tmps;
                                HttpContext.Current.Application.UnLock();
                            }
                            else
                            {
                                i = j + 1;
                                HttpContext.Current.Application.Lock();
                                if (ApplicationContent.IndexOf(Ip) > 0)
                                {
                                    string tmps = ApplicationContent.Substring(0, ApplicationContent.IndexOf(Ip));
                                    HttpContext.Current.Application[ApplicationItem] = tmps;
                                    tmps = ApplicationContent.Substring(ApplicationContent.IndexOf(Ip) + IpLength + TimeLength + 2);
                                    HttpApplicationState application2;
                                    string name2;
                                    (application2 = HttpContext.Current.Application)[name2 = ApplicationItem] = application2[name2] + tmps;
                                }
                                else
                                {
                                    string tmps = ApplicationContent.Substring(IpLength + TimeLength + 2);
                                    HttpContext.Current.Application[ApplicationItem] = tmps;
                                }
                                ApplicationItem = "checkip" + str + i.ToString();
                                if (HttpContext.Current.Application[ApplicationItem] != null)
                                {
                                    ApplicationContent = HttpContext.Current.Application[ApplicationItem].ToString();
                                    if (ApplicationContent.Length > 3200)
                                    {
                                        string tmps = string.Concat(new string[]
										{
											ApplicationContent.Substring(ApplicationContent.IndexOf(")") + 1),
											Ip,
											"(",
											System.DateTime.Now.ToLongTimeString(),
											")"
										});
                                        HttpContext.Current.Application[ApplicationItem] = tmps;
                                    }
                                    else
                                    {
                                        HttpContext.Current.Application[ApplicationItem] = string.Concat(new string[]
										{
											ApplicationContent,
											Ip,
											"(",
											System.DateTime.Now.ToLongTimeString(),
											")"
										});
                                    }
                                }
                                else
                                {
                                    HttpContext.Current.Application[ApplicationItem] = string.Concat(new string[]
									{
										ApplicationContent,
										Ip,
										"(",
										System.DateTime.Now.ToLongTimeString(),
										")"
									});
                                }
                                HttpContext.Current.Application.UnLock();
                            }
                            Flag = 1;
                            break;
                        }
                    }
                }
                if (Flag == 0)
                {
                    if (HttpContext.Current.Application["checkip" + str + "1"] != null)
                    {
                        if (HttpContext.Current.Application["checkip" + str + "1"].ToString().Length > 3200)
                        {
                            HttpContext.Current.Application.Lock();
                            HttpContext.Current.Application["checkip" + str + "1"] = string.Concat(new string[]
							{
								HttpContext.Current.Application["checkip" + str + "1"].ToString().Substring(HttpContext.Current.Application["checkip" + str + "1"].ToString().IndexOf(")") + 1),
								Ip,
								"(",
								System.DateTime.Now.ToLongTimeString(),
								")"
							});
                            HttpContext.Current.Application.UnLock();
                            return;
                        }
                        HttpContext.Current.Application.Lock();
                        HttpContext.Current.Application["checkip" + str + "1"] = string.Concat(new string[]
						{
							HttpContext.Current.Application["checkip" + str + "1"].ToString(),
							Ip,
							"(",
							System.DateTime.Now.ToLongTimeString(),
							")"
						});
                        HttpContext.Current.Application.UnLock();
                        return;
                    }
                    else
                    {
                        HttpContext.Current.Application.Lock();
                        HttpContext.Current.Application["checkip" + str + "1"] = Ip + "(" + System.DateTime.Now.ToLongTimeString() + ")";
                        HttpContext.Current.Application.UnLock();
                    }
                }
            }
        }

        public static void CheckNum(string str, ref int ireg)
        {
            ireg = 0;
            string Ip = HttpContext.Current.Request.UserHostAddress;
            string ApplicationItem = "checkip" + str + "10";
            if (HttpContext.Current.Application[ApplicationItem] != null)
            {
                string ApplicationContent = HttpContext.Current.Application[ApplicationItem].ToString();
                if (ApplicationContent.IndexOf(Ip) != -1)
                {
                    int IpLength = Ip.Length;
                    string TempStr = ApplicationContent.Substring(ApplicationContent.IndexOf(Ip));
                    int TimeLength = TempStr.IndexOf(")") - TempStr.IndexOf("(") - 1;
                    string OldTime = TempStr.Substring(TempStr.IndexOf("(") + 1, TimeLength);
                    System.TimeSpan timespan = System.Convert.ToDateTime(OldTime) - System.DateTime.Now;
                    if (timespan.Seconds > 900 || timespan.Seconds < 0)
                    {
                        HttpContext.Current.Application.Lock();
                        if (ApplicationContent.IndexOf(Ip) > 0)
                        {
                            string strtemp = ApplicationContent.Substring(0, ApplicationContent.IndexOf(Ip) - 1);
                            HttpContext.Current.Application[ApplicationItem] = strtemp;
                            strtemp = ApplicationContent.Substring(ApplicationContent.IndexOf(Ip) + IpLength + TimeLength + 2);
                            HttpApplicationState application;
                            string name;
                            (application = HttpContext.Current.Application)[name = ApplicationItem] = application[name] + strtemp + ApplicationContent.Substring(ApplicationContent.IndexOf(Ip) + IpLength + TimeLength + 2);
                        }
                        else
                        {
                            string strtemp = ApplicationContent.Substring(IpLength + TimeLength + 2);
                            HttpContext.Current.Application[ApplicationItem] = strtemp;
                        }
                        HttpContext.Current.Application.UnLock();
                        return;
                    }
                    ireg = 1;
                }
            }
        }
    }
}