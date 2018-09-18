//
//  iOSBridge.m
//  iOSBridge
//
//  Created by Supersonic.
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#import "iOSBridge.h"
#import <UIKit/UIKit.h>

// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

#ifdef __cplusplus
extern "C" {
#endif
    
    extern void UnitySendMessage( const char *className, const char *methodName, const char *param );
    
#ifdef __cplusplus
}
#endif

@implementation iOSBridge

char *const IRONSOURCE_EVENTS = "IronSourceEvents";

+ (iOSBridge *)start {
    static iOSBridge *instance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken,
                  ^{
                      instance = [iOSBridge new];
                  });
    
    return instance;
}

- (void)reportAppStarted {
    [ISEventsReporting reportAppStarted];
}

- (instancetype)init {
    if(self = [super init]){
        [IronSource setRewardedVideoDelegate:self];
        [IronSource setInterstitialDelegate:self];
        [IronSource setRewardedInterstitialDelegate:self];
        [IronSource setOfferwallDelegate:self];
    }
    
    return self;
}

- (void)setPluginDataWithType:(NSString *)pluginType pluginVersion:(NSString *)version pluginFrameworkVersion:(NSString *)frameworkVersion {
    [ISConfigurations configurations].plugin = pluginType;
    [ISConfigurations configurations].pluginVersion = version;
    [ISConfigurations configurations].pluginFrameworkVersion = frameworkVersion;
}

#pragma mark Base API

- (void)setAge:(NSInteger)age {
    [IronSource setAge:age];
}

- (void)setGender:(NSString *)gender {
    if([gender caseInsensitiveCompare:@"male"] == NSOrderedSame)
        [IronSource setGender:IRONSOURCE_USER_MALE];
    
    else if([gender caseInsensitiveCompare:@"female"] == NSOrderedSame)
        [IronSource setGender:IRONSOURCE_USER_FEMALE];
    
    else if([gender caseInsensitiveCompare:@"unknown"] == NSOrderedSame)
        [IronSource setGender:IRONSOURCE_USER_UNKNOWN];
}

- (void)setMediationSegment:(NSString *)segment {
    [IronSource setMediationSegment:segment];
}

- (const char *)getAdvertiserId {
    NSString *advertiserId = [IronSource advertiserId];
    
    return MakeStringCopy(advertiserId);
}

- (void)validateIntegration {
    [ISIntegrationHelper validateIntegration];
}

- (void)shouldTrackNetworkState:(BOOL)flag {
    [IronSource shouldTrackReachability:flag];
}

- (BOOL)setDynamicUserId:(NSString *)dynamicUserId {
    return [IronSource setDynamicUserId:dynamicUserId];
}

#pragma mark Init SDK

- (void)setUserId:(NSString *)userId {
    [IronSource setUserId:userId];
}

- (void)initWithAppKey:(NSString *)appKey {
    [IronSource initWithAppKey:appKey];
}

- (void)initWithAppKey:(NSString *)appKey adUnits:(NSArray<NSString *> *)adUnits {
    [IronSource initWithAppKey:appKey adUnits:adUnits];
}

#pragma mark Rewarded Video API

- (void)showRewardedVideo {
    [IronSource showRewardedVideoWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController];
}

- (void)showRewardedVideoWithPlacement:(NSString *)placementName {
    [IronSource showRewardedVideoWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController placement:placementName];
}

- (const char *) getPlacementInfo:(NSString *)placementName {
    char *res = nil;
    
    if (placementName){
        ISPlacementInfo *placementInfo = [IronSource rewardedVideoPlacementInfo:placementName];
        if(placementInfo){
            NSDictionary *dict = @{@"placement_name": [placementInfo placementName],
                                   @"reward_amount": [placementInfo rewardAmount],
                                   @"reward_name": [placementInfo rewardName]};
            
            res = MakeStringCopy([self getJsonFromDic:dict]);
        }
    }
    
    return res;
}

- (BOOL)isRewardedVideoAvailable {
    return [IronSource hasRewardedVideo];
}

- (BOOL)isRewardedVideoPlacementCapped:(NSString *)placementName {
    return [IronSource isRewardedVideoCappedForPlacement:placementName];
}

#pragma mark Rewarded Video Delegate

- (void)rewardedVideoHasChangedAvailability:(BOOL)available {
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAvailabilityChanged", (available) ? "true" : "false");
}

- (void)didReceiveRewardForPlacement:(ISPlacementInfo *)placementInfo {
    NSDictionary *dict = @{@"placement_reward_amount": placementInfo.rewardAmount,
                           @"placement_reward_name": placementInfo.rewardName,
                           @"placement_name": placementInfo.placementName};
    
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdRewarded", MakeStringCopy([self getJsonFromDic:dict]));
}

- (void)rewardedVideoDidFailToShowWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdShowFailed", [self parseErrorToEvent:error]);
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdShowFailed","");
}

- (void)rewardedVideoDidOpen {
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdOpened", "");
}

- (void)rewardedVideoDidClose {
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdClosed", "");
}

- (void)rewardedVideoDidStart {
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdStarted", "");
}

- (void)rewardedVideoDidEnd {
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdEnded", "");
}

#pragma mark Interstitial API

- (void)loadInterstitial {
    [IronSource loadInterstitial];
}

- (void)showInterstitial {
    [IronSource showInterstitialWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController];
}

- (void)showInterstitialWithPlacement:(NSString *)placementName {
    [IronSource showInterstitialWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController placement:placementName];
}

- (BOOL)isInterstitialReady {
    return [IronSource hasInterstitial];
}

- (BOOL)isInterstitialPlacementCapped:(NSString *)placementName {
    return [IronSource isInterstitialCappedForPlacement:placementName];
}

#pragma mark Interstitial Delegate

- (void)interstitialDidLoad {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdReady", "");
}

- (void)interstitialDidFailToLoadWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdLoadFailed", [self parseErrorToEvent:error]);
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdLoadFailed","");
}

- (void)interstitialDidOpen {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdOpened", "");
}

- (void)interstitialDidClose {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdClosed", "");
}

- (void)interstitialDidShow {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdShowSucceeded", "");
}

- (void)interstitialDidFailToShowWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdShowFailed", [self parseErrorToEvent:error]);
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdShowFailed","");
}

- (void)didClickInterstitial {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdClicked", "");
}

#pragma mark Rewarded Interstitial Delegate

- (void)didReceiveRewardForInterstitial {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdRewarded", "");
}

#pragma mark Offerwall API

- (void)showOfferwall {
    [IronSource showOfferwallWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController];
}

- (void)showOfferwallWithPlacement:(NSString *)placementName {
    [IronSource showOfferwallWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController placement:placementName];
}

- (void)getOfferwallCredits {
    [IronSource offerwallCredits];
}

- (BOOL)isOfferwallAvailable {
    return [IronSource hasOfferwall];
}

#pragma mark Offerwall Delegate

- (void)offerwallHasChangedAvailability:(BOOL)available {
    UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallAvailable", (available) ? "true" : "false");
}

- (void)offerwallDidShow {
    UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallOpened", "");
}

- (void)offerwallDidFailToShowWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallShowFailed", [self parseErrorToEvent:error]);
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallShowFailed", "");
}

- (void)offerwallDidClose {
    UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallClosed", "");
}

- (BOOL)didReceiveOfferwallCredits:(NSDictionary *)creditInfo {
    if(creditInfo)
        UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallAdCredited", [self getJsonFromDic:creditInfo].UTF8String);
    
    return YES;
}

- (void)didFailToReceiveOfferwallCreditsWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onGetOfferwallCreditsFailed", [self parseErrorToEvent:error]);
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onGetOfferwallCreditsFailed", "");
}

#pragma mark Helper methods

- (const char*)parseErrorToEvent:(NSError *)error{
    if (error){
        NSString* codeStr =  [NSString stringWithFormat:@"%ld", (long)[error code]];
        
        NSDictionary *dict = @{@"error_description": [error localizedDescription],
                               @"error_code": codeStr};
        
        return MakeStringCopy([self getJsonFromDic:dict]);
    }
    
    return nil;
}

- (NSString *)getJsonFromDic:(NSDictionary *)dict {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
    
    if (!jsonData) {
        NSLog(@"Got an error: %@", error);
        return @"";
    } else {
        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return jsonString;
    }
}


#pragma mark C Section


#ifdef __cplusplus
extern "C" {
#endif
    
    void CFReportAppStarted(){
        [[iOSBridge start] reportAppStarted];
    }
    
    void CFSetPluginData(const char *pluginType, const char *pluginVersion, const char *pluginFrameworkVersion){
        [[iOSBridge start] setPluginDataWithType:GetStringParam(pluginType) pluginVersion:GetStringParam(pluginVersion) pluginFrameworkVersion:GetStringParam(pluginFrameworkVersion)];
    }
    
    void CFSetAge(int age){
        [[iOSBridge start] setAge:age];
    }
    
    void CFSetGender(const char *gender){
        [[iOSBridge start] setGender:GetStringParam(gender)];
    }
    
    void CFSetMediationSegment(const char *segment){
        [[iOSBridge start] setMediationSegment:GetStringParam(segment)];
    }
    
    const char *CFGetAdvertiserId(){
        return [[iOSBridge start] getAdvertiserId];
    }
    
    void CFValidateIntegration(){
        [[iOSBridge start] validateIntegration];
    }
    
    void CFShouldTrackNetworkState(bool flag){
        [[iOSBridge start] shouldTrackNetworkState:flag];
    }
    
    bool CFSetDynamicUserId(char *dynamicUserId){
        return [[iOSBridge start] setDynamicUserId:GetStringParam(dynamicUserId)];
    }
    
    void CFSetUserId(char *userId){
        return [[iOSBridge start] setUserId:GetStringParam(userId)];
    }
    
    void CFInit(const char *appKey){
        [[iOSBridge start] initWithAppKey:GetStringParam(appKey)];
    }
    
    void CFInitWithAdUnits(const char *appKey, const char *adUnits[]){
        NSMutableArray *adUnitsArray = [NSMutableArray new];
        for (int i = 0; i < 3; i++)
        {
            if (adUnits[i])
                [adUnitsArray addObject: [NSString stringWithCString: adUnits[i] encoding:NSASCIIStringEncoding]];
        }
        
        [[iOSBridge start] initWithAppKey:GetStringParam(appKey) adUnits:adUnitsArray];
    }
    
    void CFShowRewardedVideo(){
        [[iOSBridge start] showRewardedVideo];
    }
    
    void CFShowRewardedVideoWithPlacementName(char *placementName){
        [[iOSBridge start] showRewardedVideoWithPlacement:GetStringParam(placementName)];
    }
    
    const char *CFGetPlacementInfo(char *placementName){
        return [[iOSBridge start] getPlacementInfo:GetStringParam(placementName)];
    }
    
    bool CFIsRewardedVideoAvailable(){
        return [[iOSBridge start] isRewardedVideoAvailable];
    }
    
    bool CFIsRewardedVideoPlacementCapped(char *placementName){
        return [[iOSBridge start] isRewardedVideoPlacementCapped:GetStringParam(placementName)];
    }
    
    void CFLoadInterstitial(){
        [[iOSBridge start] loadInterstitial];
    }
    
    void CFShowInterstitial(){
        [[iOSBridge start] showInterstitial];
    }
    
    void CFShowInterstitialWithPlacementName(char *placementName){
        [[iOSBridge start] showInterstitialWithPlacement:GetStringParam(placementName)];
    }
    
    bool CFIsInterstitialReady(){
        return [[iOSBridge start] isInterstitialReady];
    }
    
    bool CFIsInterstitialPlacementCapped(char *placementName){
        return [[iOSBridge start] isInterstitialPlacementCapped:GetStringParam(placementName)];
    }
    
    void CFShowOfferwall(){
        [[iOSBridge start] showOfferwall];
    }
    
    void CFShowOfferwallWithPlacementName(char *placementName){
        [[iOSBridge start] showOfferwallWithPlacement:GetStringParam(placementName)];
    }
    
    void CFGetOfferwallCredits(){
        [[iOSBridge start] getOfferwallCredits];
    }
    
    bool CFIsOfferwallAvailable(){
        return [[iOSBridge start] isOfferwallAvailable];
    }
    
#ifdef __cplusplus
}
#endif

@end
