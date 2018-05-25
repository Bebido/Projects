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

DEFINE_GUID (GUID_DEVINTERFACE_KMDFDriver3,
    0x19398286,0x6314,0x4f14,0x8a,0xaf,0xf8,0x94,0xfa,0xfb,0x17,0xda);
// {19398286-6314-4f14-8aaf-f894fafb17da}
