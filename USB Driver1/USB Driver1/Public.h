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

DEFINE_GUID (GUID_DEVINTERFACE_USBDriver1,
    0xdbbc2899,0x192a,0x4b6e,0xb4,0x42,0x90,0x40,0x82,0xa8,0x6f,0x30);
// {dbbc2899-192a-4b6e-b442-904082a86f30}
