#include "CommonSDK.h"
#include <sys/socket.h>
#include <netdb.h>
#include <arpa/inet.h>
#include <err.h>

@implementation CommonSDK

static NSString *s_callbackObjName = nil;
static NSString *s_callbackFunc = nil;
static int s_keyboardDeltaX = 0;

+ (int) GetKeyboardDeltaX
{
    return s_keyboardDeltaX;
}

//将文本复制到IOS剪贴板
+ (void) objc_copyTextToClipboard : (NSString*)text
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string = text;
}

+ (bool) objc_isNotificationPermitted
{
    UIUserNotificationSettings *mySettings = [[UIApplication sharedApplication] currentUserNotificationSettings];
    auto types = mySettings.types;
    return types > UIUserNotificationTypeNone;
}

+ (void) objc_clearApplicationIconBadgeNumber
{
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
}

+ (void) objc_imageSaved: (UIImage *) image didFinishSavingWithError:(NSError *)error   
    contextInfo: (void *) contextInfo  
{
    NSString *res = nil;
    if (error != nil) 
    {
        res = [error localizedDescription];
        NSLog(@"保存失败:%@", res);
    }
    else
    {
        res = @"success";
        NSLog(@"保存成功");
    }
    if(s_callbackObjName != nil && s_callbackFunc != nil)
    {
        UnitySendMessage([s_callbackObjName UTF8String], [s_callbackFunc UTF8String], [res UTF8String]);
    }
}

+ (bool) objc_isPhotoPermitionApplied
{
    PHAuthorizationStatus status = [PHPhotoLibrary authorizationStatus];
    return status != PHAuthorizationStatusNotDetermined;
}

+ (bool) objc_isPhotoPermitted
{
    PHAuthorizationStatus status = [PHPhotoLibrary authorizationStatus];
    return status == PHAuthorizationStatusAuthorized;
}

+ (void) objc_gotoMyApplicationSettings
{
    NSURL *url = [NSURL URLWithString:UIApplicationOpenSettingsURLString];
    if([[UIApplication sharedApplication] canOpenURL:url])
    {
        [[UIApplication sharedApplication] openURL:url];
    }
}

+ (bool) objc_checkCanOpenIOSURL : (NSString*)strURL
{
	 NSURL *url = [NSURL URLWithString:strURL];
	 if ([[UIApplication sharedApplication] canOpenURL:url])
	 {
		 return true;
	 }
	 else
	 {
		 return false;
	 }
}

+ (NSString *) objc_getBundleShortVersionString
{
    return [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleShortVersionString"];
}

+ (NSString *) objc_getBundleVersion
{
    return [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleVersion"];
}

extern "C"
{
    void _commsdk_init(const char *strCallbackObjName)
    {
        s_callbackObjName = [NSString stringWithUTF8String:strCallbackObjName];
    }

    void _copyTextToClipboard(const char *textList)
    {   
        NSString *text = [NSString stringWithUTF8String: textList] ;    
        [CommonSDK objc_copyTextToClipboard: text];
    }
    
    bool _isNotificationPermitted()
    {
        return [CommonSDK objc_isNotificationPermitted];
    }
    
    void _clearApplicationIconBadgeNumber()
    {
        [CommonSDK objc_clearApplicationIconBadgeNumber];
    }

    void _saveTextureToPhoto(const char *strReadAddr, const char *strCallbackFunc)
    {
        s_callbackFunc = [NSString stringWithUTF8String:strCallbackFunc];
        NSString *readAddr = [NSString stringWithUTF8String:strReadAddr];
        UIImage *img = [UIImage imageWithContentsOfFile:readAddr];
        NSLog(@"%@", [NSString stringWithFormat:@"w:%f, h:%f", img.size.width, img.size.height]);
        UIImageWriteToSavedPhotosAlbum(img, CommonSDK.class,
            @selector(objc_imageSaved:didFinishSavingWithError:contextInfo:), nil);
    }

    bool _isPhotoPermitionApplied()
    {
        return [CommonSDK objc_isPhotoPermitionApplied];
    }

    bool _isPhotoPermitted()
    {
        return [CommonSDK objc_isPhotoPermitted];
    }

    void _gotoMyApplicationSettings()
    {
        [CommonSDK objc_gotoMyApplicationSettings];
    }
	
	bool _checkCanOpenIOSURL(const char *strUrl)
	{
		return [CommonSDK objc_checkCanOpenIOSURL:[NSString stringWithUTF8String:strUrl]];
	}

    #define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

    void _setMainWindowFrame(int x, int y, int width, int height)
    {
        UIWindow* mainWindow = UnityGetMainWindow();
        if(mainWindow != NULL)
        {
            CGRect winFrame = CGRectMake(x, y, width, height);
            [mainWindow setFrame:winFrame];
        }
    }
    
    void _setMainWindowFrameDelta(int deltaX, int deltaY, int deltaWidth, int deltaHeight)
    {
        UIWindow* mainWindow = UnityGetMainWindow();
        if(mainWindow != NULL)
        {
            CGRect winFrame = [mainWindow frame];
            winFrame.origin.x += deltaX;
            winFrame.origin.y += deltaY;
            winFrame.size.width += deltaWidth;
            winFrame.size.height += deltaHeight;
            [mainWindow setFrame:winFrame];
        }
    }
    
    void _setKeyboardDeltaX(int deltaX)
    {
        s_keyboardDeltaX = deltaX;
    }

    const char *_getBundleShortVersionString()
    {
        NSString *str = [CommonSDK objc_getBundleShortVersionString];
        return MakeStringCopy(str);
    }

    const char *_getBundleVersion()
    {
        NSString *str = [CommonSDK objc_getBundleVersion];
        return MakeStringCopy(str);
    }
}
@end
