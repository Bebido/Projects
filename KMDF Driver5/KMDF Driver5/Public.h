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
// Define an Interface Guid so that apps can find the device and talk to it.
//

DEFINE_GUID (GUID_DEVINTERFACE_KMDFDriver5,
    0xf1291a6a,0xbef2,0x447d,0xaa,0xe6,0xa5,0x73,0xc9,0x9b,0x98,0xbf);
// {f1291a6a-bef2-447d-aae6-a573c99b98bf}
