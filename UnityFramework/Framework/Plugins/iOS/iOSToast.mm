//
//  UnityNative_Toasts.mm
//
//  Created by Nicholas Sheehan on 01/06/2018.
//

#import "UnityNative_Toasts.h"
#import "ToastView.h"

inline void iOS_ShowToast(const char* toastText)
{
    NSString *toastMessage = [NSString stringWithUTF8String:toastText];
    
    UIViewController* rootViewController = UnityGetGLViewController();
    
    UIView* view = rootViewController.view;
    
    [ToastView showToastInParentView:view withText:toastMessage withDuaration:2.0f];
}

