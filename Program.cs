using System;

Interpreter interpreter = new Interpreter();

string destination = RemoveSpaces("00000000 00000000 00000000 00000000 00000000 00000000"); //six octets
string source = RemoveSpaces("11110000 00001111 11001100 11110000 00001111 11001100"); //six octets
string etherType = RemoveSpaces("00001000 00000000"); //0x0800 signifies IPv4
string payload = RemoveSpaces("00001111 00001111 00001111 00001111"); // can be whatever


EthernetFrame frame = new EthernetFrame(destination, source, etherType, payload);

Console.WriteLine(frame.BinaryData);
Console.WriteLine(frame.BinaryData.Length/8);


/*
string binaryData = RemoveSpaces("10101010 10101010 10101010 10101010 10101011 10101010");
Console.WriteLine(binaryData.Length);
interpreter.Interpret(binaryData);
Console.WriteLine("inPreamble: " + interpreter.inPreamble);
Console.WriteLine("sfdReached: " + interpreter.sfdReached);
*/

string RemoveSpaces(string input)
{
    string output = "";

    for(int i = 0; i < input.Length; i++)
    {
        char c = input[i];
        if (c != ' ')
        {
            output += c;
        }
    }

    return output;
}

public class EthernetFrame
{
    public string BinaryData { get; private set; } = "";
    public string Preamble { get; } = "1010101010101010101010101010101010101010101010101010101010101011";
    public string DestinationMAC { get; init; }
    public string SourceMAC { get; init; }
    public string EtherType { get; init; }
    public string Payload { get; init; }
    
    public EthernetFrame(string destinationMAC, string sourceMAC, string etherType, string payload)
    {
        DestinationMAC = destinationMAC;
        SourceMAC = sourceMAC;
        EtherType = etherType;
        Payload = payload;
        string padding = "";

        if (payload.Length < (42 * 8))
        {
            int paddingAmount = (42 * 8) - payload.Length;
            for (int i = 0; i < paddingAmount; i++)
            {
                padding += 0;
            }
        }


        BinaryData = Preamble + destinationMAC + sourceMAC + etherType + payload + padding;
    }

}

class Interpreter
{
    public bool inPreamble { get; private set; } = false;
    public bool sfdReached { get; private set; } = false;
    public string currentOctet { get; private set; } = "";

    public void Interpret(string binaryData)
    {
        for (int i = 0; i < binaryData.Length; i++)
        {
            
            if (currentOctet.Length == 8)
            {
                PreambleCheck(currentOctet);
                currentOctet = "";
            }
            currentOctet += binaryData[i];

            Console.WriteLine($"{i}: {currentOctet}");
        }
    }

    void PreambleCheck(string octet)
    {
        if (octet == "10101010")
            inPreamble = true;

        if (inPreamble && octet == "10101011")
        {
            inPreamble = false;
            sfdReached = true;
        }
    }


}