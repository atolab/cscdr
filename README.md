[![License](https://img.shields.io/badge/License-EPL%202.0-blue)](https://choosealicense.com/licenses/epl-2.0/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

# CSCDR

A DDS-CDR encoder/decoder library in C#.

CDR stands for the O.M.G.'s Common Data Representation that is used in DDS (Data Distributed Service) implementations.
It's specified by https://www.omg.org/spec/DDSI-RTPS/2.3/PDF (chapter 10) and https://www.omg.org/cgi-bin/doc?formal/02-06-51.

In the current version, this library only handles the basic types.
It doesn't include an IDL compiler generating C# encoder/decoder for complex types.

## Usage

Assuming a DDS type is defined with the following IDL:
```c
module HelloWorldData
{
  struct Msg
  {
    long userID;
    string message;
  };
  #pragma keylist Msg userID
};
```

The code to encode such type will be:
```csharp
HelloWorldData.Msg m = new HelloWorldData.Msg();
m.userId = 1;
m.message = "Hello World!";

CDRWriter writer = new CDRWriter();
writer.WriteInt32(m.userId);
writer.WriteString(m.message);

byte[] encodedBuffer = writer.GetBuffer().ToArray();
```

The code to decode such type will be:
```csharp
byte[] encodedBuffer = ...;

CDRReader reader = new CDRReader(encodedBuffer);
HelloWorldData.Msg m = new HelloWorldData.Msg();
m.userId = reader.ReadInt32();
m.message = reader.ReadString();
```
