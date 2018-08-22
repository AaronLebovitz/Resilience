#pragma clang diagnostic ignored "-Wdeprecated-declarations"
#pragma clang diagnostic ignored "-Wtypedef-redefinition"
#pragma clang diagnostic ignored "-Wobjc-designated-initializers"
#include <stdarg.h>
#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>
#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>
#import <CloudKit/CloudKit.h>

@class NSComboBoxDataSource;
@class NSApplicationDelegate;
@protocol NSMenuValidation;
@class __monomac_internal_ActionDispatcher;
@class NSURLSessionDataDelegate;
@class __MonoMac_NSActionDispatcher;
@class __MonoMac_NSAsyncActionDispatcher;
@class ResilienceClasses_ComboBoxStringListDataSource;
@class __NSGestureRecognizerToken;
@class __NSClickGestureRecognizer;
@class __NSGestureRecognizerParameterlessToken;
@class __NSGestureRecognizerParametrizedToken;
@class __NSMagnificationGestureRecognizer;
@class __NSPanGestureRecognizer;
@class __NSPressGestureRecognizer;
@class __NSRotationGestureRecognizer;
@class Foundation_NSUrlSessionHandler_WrappedNSInputStream;
@class __NSObject_Disposer;
@class Foundation_NSUrlSessionHandler_NSUrlSessionHandlerDelegate;
@class AppDelegate;
@class ViewController;

@interface NSComboBoxDataSource : NSObject<NSComboBoxDataSource> {
}
	-(id) init;
@end

@interface NSApplicationDelegate : NSObject<NSApplicationDelegate> {
}
	-(id) init;
@end

@protocol NSMenuValidation
	@required -(BOOL) validateMenuItem:(NSMenuItem *)p0;
@end

@interface NSURLSessionDataDelegate : NSObject<NSURLSessionDataDelegate, NSURLSessionTaskDelegate, NSURLSessionDelegate> {
}
	-(id) init;
@end

@interface ResilienceClasses_ComboBoxStringListDataSource : NSObject<NSComboBoxDataSource> {
}
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(NSString *) comboBox:(NSComboBox *)p0 completedString:(NSString *)p1;
	-(NSInteger) comboBox:(NSComboBox *)p0 indexOfItemWithStringValue:(NSString *)p1;
	-(NSInteger) numberOfItemsInComboBox:(NSComboBox *)p0;
	-(NSObject *) comboBox:(NSComboBox *)p0 objectValueForItemAtIndex:(NSInteger)p1;
	-(BOOL) conformsToProtocol:(void *)p0;
@end

@interface __NSGestureRecognizerToken : NSObject {
}
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(BOOL) conformsToProtocol:(void *)p0;
@end

@interface __NSGestureRecognizerParameterlessToken : __NSGestureRecognizerToken {
}
	-(void) target;
@end

@interface __NSGestureRecognizerParametrizedToken : __NSGestureRecognizerToken {
}
	-(void) target:(NSGestureRecognizer *)p0;
@end

@interface AppDelegate : NSObject<NSApplicationDelegate> {
}
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(void) applicationDidFinishLaunching:(NSNotification *)p0;
	-(void) applicationWillTerminate:(NSNotification *)p0;
	-(BOOL) conformsToProtocol:(void *)p0;
	-(id) init;
@end

@interface ViewController : NSViewController {
}
	@property (nonatomic, assign) NSPopUpButton * LenderPopUpButton;
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(NSPopUpButton *) LenderPopUpButton;
	-(void) setLenderPopUpButton:(NSPopUpButton *)p0;
	-(void) viewDidLoad;
	-(NSObject *) representedObject;
	-(void) setRepresentedObject:(NSObject *)p0;
	-(void) LenderSelected:(NSPopUpButton *)p0;
	-(void) RunReportPressed:(NSButton *)p0;
	-(BOOL) conformsToProtocol:(void *)p0;
@end


