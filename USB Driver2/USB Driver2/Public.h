/*++

Module Name:

    public.h

Abstract:

    This module contains the common declarations shared by driver
    and user applications.

Environment:

    user and kernel

--*/

//
// Define an Interface Guid so that app can find the device and talk to it.
//

DEFINE_GUID (GUID_DEVINTERFACE_USBDriver2,
    0xb1811688,0xfd73,0x4dcb,0xaf,0x67,0x1e,0xb5,0x64,0xfd,0xb3,0xea);
// {b1811688-fd73-4dcb-af67-1eb564fdb3ea}
