/* Program przyk³adowy ilustrujcy operacje FMA z
wykorzystaniem instrukcji AVX procesora
Program jest przystosowany do wspó³pracy z podprogramem
zakodowanym w asemblerze (plik funkcjeAVX.asm)
*/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <Windows.h>
#include <time.h>

extern "C" int FMA(float * matA, float * matB, float scalar, int count);
const __int64 DELTA_EPOCH_IN_MICROSECS = 11644473600000000;
#define FLOPS_ARRAY_SIZE (1024*1024)
#define MAXFLOPS_ITERS 100000000
#define LOOP_COUNT 128
#define FLOPSPERCALC 2
__declspec(align(64)) float fa[FLOPS_ARRAY_SIZE];
__declspec(align(64)) float fb[FLOPS_ARRAY_SIZE];
struct timezone2
{
	__int32 tz_minuteswest;
	bool tz_dsttime;
};
struct timeval2 {
	__int32 tv_sec; /* seconds */
	__int32 tv_usec; /* microseconds */
};
int gettimeofday(struct timeval2 *tv, struct timezone2 *tz)
{
	FILETIME ft;
	__int64 tmpres = 0;
	TIME_ZONE_INFORMATION tz_winapi;
	int rez = 0;
	ZeroMemory(&ft, sizeof(ft));
	ZeroMemory(&tz_winapi, sizeof(tz_winapi));
	GetSystemTimeAsFileTime(&ft);
	tmpres = ft.dwHighDateTime;
	tmpres <<= 32;
	tmpres |= ft.dwLowDateTime;
	tmpres /= 10;
	tmpres -= DELTA_EPOCH_IN_MICROSECS;
	tv->tv_sec = (__int32)(tmpres*0.000001);
	tv->tv_usec = (tmpres % 1000000);
	rez = GetTimeZoneInformation(&tz_winapi);
	tz->tz_dsttime = (rez == 2) ? true : false;
	tz->tz_minuteswest = tz_winapi.Bias +
		((rez == 2) ? tz_winapi.DaylightBias : 0);
	return 0;
}
double dtime()
{
	double tseconds = 0.0;
	struct timeval2 mytime;
	struct timezone2 myzone;
	gettimeofday(&mytime, &myzone);
	tseconds =
		(double)(mytime.tv_sec + mytime.tv_usec*1.0e-6);
	return tseconds;
}
int main(int argc, char *argv[])
{
	/* celem programu jest obliczenie czasu wykonania
	operacji FMA dla dwóch macierzy fa i fb
	o wymiarach 1024x1024 ( FLOPS_ARRAY_SIZE)
	liczba powtórze oblicze wynosi
	MAXFLOPS_ITERS 100000000
	*/
	int i, j, k;
	double tstart, tstop, ttime;
	double gflops = 0.0;
	float a = 2.0;
	printf("Inicjalizacja \r\n");
	// wype³nienie tablicy fa i fb pewnymi wartociami
	for (i = 0; i < FLOPS_ARRAY_SIZE; i++)
	{
		fa[i] = (float)i + 0.1f;
		fb[i] = (float)i + 0.2f;
	}
	printf("Pocztek obliczeMAXFLOPS_ITERS \n");
	tstart = dtime();
	// MAXFLOPS_ITERS
	for (j = 0; j < MAXFLOPS_ITERS; j++)
	{
		if (FMA(fa, fb, a, LOOP_COUNT) != 0) exit(0);
		// obliczenie wartoci z wykorzystaniem instrukcji
		// AVX2 fa = a*fa + fb
		/*
		Ten komentarz zawiera odpowiednik funkcji FMA w
		jzyku C
		*/
		for (k = 0; k < LOOP_COUNT; k++)
		{
			fa[k] = a*fa[k] + fb[k];
		}
	}
	tstop = dtime();
	gflops = (double)(1.0e-9 * LOOP_COUNT * MAXFLOPS_ITERS *
		FLOPSPERCALC);
	ttime = tstop - tstart;
	if (ttime > 0.0)
	{
		printf("GFlops = %10.3lf, secs =%10.2lf\n", gflops,
			ttime);
	}
	return 0;
}