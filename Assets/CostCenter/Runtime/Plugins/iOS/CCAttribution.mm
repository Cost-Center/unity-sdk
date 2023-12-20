#import <AdServices/AdServices.h>

char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

extern "C" {
	
	const char* _GetAttributionToken()
	{
        if (@available(iOS 14.3, *)) {
            NSError *error;
            NSString *attributionToken = [AAAttribution attributionTokenWithError:&error];
            return MakeStringCopy([attributionToken UTF8String]);
        }
        return NULL;
	}
	
}
