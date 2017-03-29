namespace Dorado.ESB.Common.Config
{
    public class LR
    {
        private LR()
        {
        }

        #region Dorado.ESB.ServiceHost_config

        public static string PlatformServices_ServiceHost_Config = @"
         <configuration>
          <system.diagnostics/>

          <system.serviceModel>
            <bindings>
                <basicHttpBinding>
                    <binding name='BasicHttpBinding_DoradoPlatformServices' />
                </basicHttpBinding>
               <netTcpBinding>
                <binding name='CustomTcpBinding_DoradoPlatformServices' closeTimeout='00:01:00'
                 openTimeout='00:01:00' receiveTimeout='01:00:00' sendTimeout='00:01:00'
                 transactionFlow='false' transferMode='Buffered' transactionProtocol='OleTransactions'
                 hostNameComparisonMode='StrongWildcard' listenBacklog='1000'
                 maxBufferPoolSize='524288' maxBufferSize='524288' maxConnections='200'
                 maxReceivedMessageSize='524288'>
                 <readerQuotas maxDepth='64' maxStringContentLength='131072' maxArrayLength='16384'
                  maxBytesPerRead='16384' maxNameTableCharCount='16384' />
                 <reliableSession ordered='true' inactivityTimeout='00:01:00'
                  enabled='false' />
                 <security mode='None' />
                </binding>
               </netTcpBinding>
               <webHttpBinding>
                <binding name='webHttpBinding_DoradoPlatformServices'>
                 <readerQuotas maxDepth='64' maxArrayLength='16384' maxBytesPerRead='16384' />
                </binding>
               </webHttpBinding>
            </bindings>
		    <services>
			<service behaviorConfiguration='DoradoPlatformServiceBehaviors'
                                    name='Dorado.ESB.Library.ApiWcf'>
				<host>
					<baseAddresses>
						<add baseAddress='http://localhost:8000/DoradoPlatformService'/>
					</baseAddresses>
				</host>
				<endpoint address='http://localhost:8000/DoradoPlatformService'
     binding='basicHttpBinding' bindingConfiguration='BasicHttpBinding_DoradoPlatformServices'
     name='Http_DoradoPlatformServices' contract='Dorado.ESB.Library.ServiceContracts.IApiZhaoPinWcf' />
				<endpoint address='net.tcp://localhost:8001/DoradoPlatformService'
     binding='netTcpBinding' bindingConfiguration='CustomTcpBinding_DoradoPlatformServices'
     name='NetTcp_DoradoPlatformServices' contract='Dorado.ESB.Library.ServiceContracts.IApiZhaoPinWcf' />
			</service>
		</services>

		<behaviors>
			<serviceBehaviors>
				<behavior name='DoradoPlatformServiceBehaviors'>
					<serviceDebug httpHelpPageEnabled='true' includeExceptionDetailInFaults='true' />
					<serviceMetadata httpGetEnabled='true' httpGetUrl='' />
					<serviceThrottling maxConcurrentInstances='1000' maxConcurrentCalls='1000' maxConcurrentSessions='1000' />
				</behavior>
			</serviceBehaviors>
		</behaviors>

	</system.serviceModel>
        </configuration>";

        #endregion Dorado.ESB.ServiceHost_config
    }
}