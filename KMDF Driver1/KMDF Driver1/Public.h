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

DEFINE_GUID (GUID_DEVINTERFACE_KMDFDriver1,
    0xb7784e63,0x9fb6,0x412d,0xbb,0x2a,0x11,0xbc,0x28,0x13,0x4d,0x44);
// {b7784e63-9fb6-412d-bb2a-11bc28134d44}
