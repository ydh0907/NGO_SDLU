#pragma comment(lib, "Ws2_32_lib")
#include <iostream>
#include <WinSock2.h>

using namespace std;

int main() {
	WSAStartup();

	SOCKET server = INVALID_SOCKET;

	server = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (server == INVALID_SOCKET)
		return 1;

	cout << server;

	int result = closesocket(server);
}