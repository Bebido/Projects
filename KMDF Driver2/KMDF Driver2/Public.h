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

DEFINE_GUID (GUID_DEVINTERFACE_KMDFDriver2,
    0x98e5ba30,0xaf2b,0x4429,0x9b,0x91,0x04,0x23,0xf9,0x76,0x68,0xeb);
// {98e5ba30-af2b-4429-9b91-0423f97668eb}
