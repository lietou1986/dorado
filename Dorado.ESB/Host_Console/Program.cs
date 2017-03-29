using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beisen.ESB.Core;
using Beisen.ESB.Common.Config;
using Beisen.ESB.Core.Contracts;
using System.Threading;

namespace Host_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Press anykey to start");
                Console.ReadLine();

                //+++++++ this is a major function ++++++++
                HostServices.Current.Boot();
                //HostServices.Current.Boot("HelloWorldApp.Library");

                //+++++++++++++++++++++++++++++++++++++++++


                #region Show loaded services
                Console.WriteLine("\n------- Loaded Services into appDomains----------------------");
                Console.WriteLine("\n------------Host Name " + HostServices.Current.HostName + "-----------------");
                
                foreach (ServiceMetadataBase hostedService in HostServices.Current.GetHostedServices)
                {
                    Console.WriteLine("[{0}] {1}", hostedService.AppDomainHostName, hostedService.Name);
                }
                #endregion

                #region add some handlers
                #endregion

                //+++++++ open all services ++++++++
                Console.WriteLine("\n------- Opening Services -------------------------------------\n");
                HostServices.Current.Open();
                //+++++++++++++++++++++++++++++++++++++++++


                //Console.WriteLine("\n------- existed Services into appDomains----------------------");
                //List<ServiceMetadataBase> serviceMetadataBaseList = new List<ServiceMetadataBase>();
                //Beisen.ESB.Core.DomainLoader domain = new DomainLoader();
    
                //serviceMetadataBaseList = domain.GetHostedServices("API_BeisenPlatformServices1");
                //foreach (ServiceMetadataBase hostedService in serviceMetadataBaseList)
                //{
                //    Console.WriteLine("[{0}] {1}", hostedService.AppDomainHostName, hostedService.Name);
                //}



                //Console.WriteLine("\npress any key to close services ...\n");
                //Console.ReadLine();

                //Console.WriteLine("\n------- UnloadDomain Services into appDomains----------------------");

                //domain.UnloadDomain("API_BeisenPlatformServices1", "Beisen.ESB.HelloWorldApp");

                //Console.WriteLine("\n------- existed Services into appDomains----------------------");

                //serviceMetadataBaseList = domain.GetHostedServices("API_BeisenPlatformServices1");
                //foreach (ServiceMetadataBase hostedService in serviceMetadataBaseList)
                //{
                //    Console.WriteLine("[{0}] {1}", hostedService.AppDomainHostName, hostedService.Name);
                //}


                // clean-up process
                AppDomain.CurrentDomain.ProcessExit += delegate
                {
                    Console.WriteLine("[{0}] Process shutdown ...", AppDomain.CurrentDomain.FriendlyName);
                    HostServices.Current.Close();
                };

                Thread.Sleep(2000);
                Console.WriteLine("\npress any key to close services ...\n");
                Console.ReadLine();
  
            }          
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
    }
}
