# MultiplayerChess
Multiplayer Chess game in C# made for class

Packets sent by the server
SWHO|<Name1>|<Name2|... - request name, send list of connected client
SCNN|<Name> - somebody connected
SMOV|<Name>|x|y|x|y - <name> moves piece from xy to xy
SLOS|<NAME> - <name> has lost


Pakiety wysyłane przez klienta:
CWHO|<Name>|<isHost> - is <name> host
CMOV|<Name>|x|y|x|y - <name> moves piece from xy to xy
CLOS|<NAME> - <name> has lost



