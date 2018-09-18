author: Djordje Dozet
date:   22.02.2016. 

Build Instructions:

1)add libraries:

- AdSupport.framework 					(req. by googleAnalytics)
- CoreData.framework 					(req. by googleAnalytics)
- SystemConfiguration.framework 		(req. by googleAnalytics)
- libz.dylib 							(req. by googleAnalytics)
- libsqlite3.dylib 						(req. by googleAnalytics)
- Social.framework
- Accounts.framework
- MessageUI.framework
- MobileCoreSevices.framework
- GameKit.framework						
- eventkit.framework
- eventkitUI.framework
- googleMobileAds.framework				(not required if already imported)

2)build settings:

- enable modules: YES
- link frameworks automatically: YES
- enable bitcode: NO

3)Build Phases/Compile Sources:

- FbUnityUtility.mm double click
- Add (write down) compiler flag: "-fno-objc-arc" (without "")
