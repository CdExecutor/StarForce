#import <Foundation/Foundation.h>
#import <Photos/Photos.h>

@interface Network : NSObject

extern "C"
{
    //返回主机地址&&地址类型（根据指定ipv4地址以及当前环境）
    //如192.168.1.1&&ipv4
    const char *_getHostInfo(const char *strSourceHost);
}
@end
