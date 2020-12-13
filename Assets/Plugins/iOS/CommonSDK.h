#import <Foundation/Foundation.h>
#import <Photos/Photos.h>

@interface CommonSDK : NSObject
+ (int) GetKeyboardDeltaX;

+ (void) objc_copyTextToClipboard : (NSString*)text;

+ (bool) objc_isNotificationPermitted;

+ (void) objc_clearApplicationIconBadgeNumber;

+ (void) objc_imageSaved: (UIImage *) image didFinishSavingWithError:(NSError *)error
    contextInfo: (void *) contextInfo;

+ (bool) objc_isPhotoPermitionApplied;

+ (bool) objc_isPhotoPermitted;

+ (void) objc_gotoMyApplicationSettings;

+ (bool) objc_checkCanOpenIOSURL : (NSString *) strUrl;

+ (NSString *) objc_getBundleShortVersionString;

+ (NSString *) objc_getBundleVersion;

extern "C"
{
    void _commsdk_init(const char *strCallbackObjName);
    void _copyTextToClipboard(const char *textList);
    bool _isNotificationPermitted();
    void _clearApplicationIconBadgeNumber();
    //保存图片到相册
    void _saveTextureToPhoto(const char *strReadAddr, const char *strCallbackFunc);
    //相册权限是否已经申请过了
    bool _isPhotoPermitionApplied();
    //是否有相册权限
    bool _isPhotoPermitted();
    //跳转到当前程序的设置
    void _gotoMyApplicationSettings();
	//检测是否能打开iosurl
	bool _checkCanOpenIOSURL(const char *strUrl);

    //返回主机地址&&地址类型（根据指定ipv4地址以及当前环境）
    //如192.168.1.1&&ipv4

    void _setMainWindowFrame(int x, int y, int width, int height);
    void _setMainWindowFrameDelta(int deltaX, int deltaY, int deltaWidth, int deltaHeight);

    const char *_getBundleShortVersionString();
    const char *_getBundleVersion();
}
@end
