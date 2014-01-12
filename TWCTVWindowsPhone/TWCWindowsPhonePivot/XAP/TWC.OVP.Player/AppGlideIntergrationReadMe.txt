AppGlide plug-in integration.

1. Introduction
	AppGlide is a statistic data collection/analyzing system developed by Alcatel-Lucent. 
	It consists of a client plugin for SMFPlayer, and an analytic server hardware. It collects data from player and reports them to analytic server. AppGlide plug-in is developed following MMPPF genetic plug-in design patter. 
	This document describes AppGlide integration and configuration in TWC.OVP client project. 


2. Adding/Removing AppGlide.
	The plug-in comes with two files:
		1. “AppglideSilverlightPlugin.dll” (plug-in dll file)
		2. “XapPlugIns.xml” (client configuration file)
	To add AppGlide:
		1.	Reference AppglideSilverlightPlugin.dll in TWC.OVP project. 
		2.	Add XapPlugIns.xml into TWC.OVP project at its root folder.
		3.	Set “Build Action” for XapPlugIns.xml to “Content”.
	To remove AppGlide:
		Remove the reference to “AppglideSilverlightPlugin.dll” and file “XapPlugIns.xml” from TWC.OVP project.


3. Configuring AppGlide.
    AppGlide contains:
	  1. Client configuration file, which configures server configuration url, waiting time, debug output flag.
	  2. Server configuration, which is hosted on the analytic server, configures data collecting service endpoints and other parameters.

	1. Client configuration.
		<appSettings>
			<add key="AppglideConfigFileUrl" value="http://cfg.ag.timewarnercable.com:8080/cfg/sl/20161882_4b7c4bccc7bb7dac2bbfc25272a77e5f" /> -- Server configuration URL.
			<add key="AppglideWaitThreshold" value="5"/> -- Initial waiting time before successfully downloading information from server. If plug-in failed to initialize itself within the time, it will be disabled in the session.
			<add key="AppglideInitialDebug" value="false" /> --Turn on/off Browser console debug output.
		</appSettings>

	2. Server configuration(Out of scope, list here for reference)
		<?xml version="1.0" encoding="UTF-8"?>
		<xml>
			<measurementWS>
			  <endpoint>http://meas.ag.timewarnercable.com:8080/wsrest/analyticsPluginService/addMeasurements</endpoint> //measurement data collecting service endpoint
			</measurementWS>
			<criticalWS>
			  <endpoint>http://fail.ag.timewarnercable.com:8080/rta-ws/realTimeEvent/addEvent?app=appglide&stream=error</endpoint> //error log collecting service endpoint
			</criticalWS>
			<reportInterval>20000</reportInterval>
			<initialBufferTimeout>10000</initialBufferTimeout>
			<longRebufferingThreshold>5000</longRebufferingThreshold>
			<componentName>twc_pl_1</componentName>
			<reportingEnabled>true</reportingEnabled>
		</xml>


4. Contacts:
	TWCTV:
		Kaminetsky, Joshua joshua.kaminetsky@twcable.com  OVP Lead
		Zheng, Kelvin (contractor) kelvin.zheng@twc-contractor.com  Developer

	Alcatel Lucent:
		Conwell, Kent T (Kent) kent.conwell@alcatel-lucent.com Consultant
		Janssens, Nico (Nico) nico.janssens@alcatel-lucent.com Developer
		Migdisoglu, Murat (Murat) murat.migdisoglu@alcatel-lucent.com  Developer

