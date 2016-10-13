using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBDzad2
{

    public static class global
    {
        public static uint licznikOdczytow = 0;
        public static uint licznikZapisow = 0;
        public static uint licznikSplit = 0;
        public static uint licznikKom = 0;
        public static uint licznikMerge = 0;
        public static int rozmiarStrony = 100;
        public static List<int> indeksy = new List<int>();
        public static List<int> listaH = new List<int>();
    }
    unsafe struct rawRekord
    {
        public int indeks;
        public fixed byte liczby[5];
    }
    class Rekord : IComparable
    {
        public rawRekord rekord;
        private double srednia;

        unsafe public Rekord(int dIndeks, byte liczba1, byte liczba2, byte liczba3, byte liczba4, byte liczba5)
        {
            int suma = liczba1 + liczba2 + liczba3 + liczba4 + liczba5;
            fixed (rawRekord* p = &this.rekord)
            {
                p->indeks = dIndeks;
                p->liczby[0] = liczba1;
                p->liczby[1] = liczba2;
                p->liczby[2] = liczba3;
                p->liczby[3] = liczba4;
                p->liczby[4] = liczba5;
            }
            this.srednia = suma / 5.0;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            else
            {
                Rekord inny = (Rekord)obj;
                return this.srednia.CompareTo(inny.srednia);
            }
        }

        unsafe public void wypisz()
        {
            fixed (rawRekord* p = &this.rekord)
            {
                //Console.WriteLine("{0}\t {1}\t {2}\t {3}\t {4}\t {5}", p->indeks, p->liczby[0], p->liczby[1], p->liczby[2], p->liczby[3], p->liczby[4], this.srednia);
                Console.Write("{0}\t", p->indeks);
            }
        }
    }

    class Tasma
    {
        private string sciezka;

        public Tasma(string dSciezka, bool dInput = true, bool dLicz = true)
        {
            this.sciezka = dSciezka;
        }

        unsafe private Tuple<List<Rekord>, List<int>, int> odczytajRekordy(byte[] strona, int ilosc, int pozycja)
        {
            Tuple<List<Rekord>, List<int>, int> rekordy = new Tuple<List<Rekord>, List<int>, int>(new List<Rekord>(), new List<int>(), pozycja);
            int numerBajtu = 0;
            int wskaznik = (int)(strona[numerBajtu++] | strona[numerBajtu++] << 8 | strona[numerBajtu++] << 16 | strona[numerBajtu++] << 24);
            rekordy.Item2.Add(wskaznik);
            for (; (numerBajtu + 9 + sizeof(int) < global.rozmiarStrony) && (numerBajtu < ilosc);)
            {
                Rekord nowy = new Rekord((int)(strona[numerBajtu++] | strona[numerBajtu++] << 8 | strona[numerBajtu++] << 16 | strona[numerBajtu++] << 24), strona[numerBajtu++], strona[numerBajtu++], strona[numerBajtu++], strona[numerBajtu++], strona[numerBajtu++]);
                if (nowy.rekord.indeks == -2)
                    break;
                rekordy.Item1.Add(nowy);
                wskaznik = (int)(strona[numerBajtu++] | strona[numerBajtu++] << 8 | strona[numerBajtu++] << 16 | strona[numerBajtu++] << 24);
                rekordy.Item2.Add(wskaznik);
            }
            return rekordy;
        }

        public Tuple<List<Rekord>, List<int>, int> wczytajStrone(int pozycja = 0)
        {
            global.licznikOdczytow++;
            FileStream stream = File.OpenRead(sciezka);
            byte[] strona = new byte[global.rozmiarStrony];
            stream.Position = pozycja;
            int ilosc = stream.Read(strona, 0, global.rozmiarStrony);
            stream.Close();
            stream.Dispose();
            if (ilosc != 0)
            {
                return this.odczytajRekordy(strona, ilosc, pozycja);
            }
            else
            {
                return new Tuple<List<Rekord>, List<int>, int>(new List<Rekord>(), new List<int>(), pozycja);
            }
        }

        unsafe private byte[] zakodujRekordy(Tuple<List<Rekord>, List<int>, int> rekordy)
        {
            byte[] bajty = new byte[global.rozmiarStrony];
            for (int i = 0; i < global.rozmiarStrony; i++)
            {
                bajty[i] = 0;
            }
            int numerBajtu = 0;
            if (rekordy.Item2.Count == 0)
            {
                for (int j = 0; j < 4; j++)
                {
                    bajty[numerBajtu++] = 0;
                }
            }
            else
            {
                byte[] wsk = BitConverter.GetBytes(rekordy.Item2[0]);
                for (int j = 0; j < 4; j++)
                {
                    bajty[numerBajtu++] = wsk[j];
                }
            }
            for (int i = 0; i < rekordy.Item1.Count; i++)
            {
                fixed (rawRekord* p = &rekordy.Item1[i].rekord)
                {
                    byte[] ind = BitConverter.GetBytes(p->indeks);
                    for (int j = 0; j < 4; j++)
                    {
                        bajty[numerBajtu++] = ind[j];
                    }
                    for (int j = 0; j < 5; j++)
                    {
                        bajty[numerBajtu++] = p->liczby[j];
                    }
                }
                if (rekordy.Item2.Count == 0)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        bajty[numerBajtu++] = 0;
                    }
                }
                else
                {
                    byte[] wsk = BitConverter.GetBytes(rekordy.Item2[i + 1]);
                    for (int j = 0; j < 4; j++)
                    {
                        bajty[numerBajtu++] = wsk[j];
                    }
                }
            }
            if (numerBajtu + sizeof(int) < global.rozmiarStrony)
            {
                byte[] koniec = BitConverter.GetBytes(-2);
                for (int i = 0; i < sizeof(int); i++)
                {
                    bajty[numerBajtu++] = koniec[i];
                }
            }
            return bajty;
        }

        unsafe public void zapiszStrone(Tuple<List<Rekord>, List<int>, int> rekordy)
        {
            global.licznikZapisow++;
            FileStream stream = File.OpenWrite(sciezka);
            stream.Position = rekordy.Item3;
            if (rekordy.Item1.Count * 9 + rekordy.Item2.Count * sizeof(int) > global.rozmiarStrony)
            {
                Console.WriteLine("Blad zapisu strony");
            }
            else
            {
                stream.Write(zakodujRekordy(rekordy), 0, global.rozmiarStrony);
            }
            stream.Close();
            stream.Dispose();
        }
    }

    class Drzewo
    {
        private Tasma tasma;
        private int d, pozycjaOstatniejStrony;
        public int h;
        private int pozycjaRoot;
        private List<Tuple<List<Rekord>, List<int>, int>> bufor;
        private List<string> doWypisywania;
        private List<int> wolneAdresy;

        public Drzewo(string sciezka)
        {
            if (File.Exists(sciezka))
            {
                File.Delete(sciezka);
            }
            var stream = File.Create(sciezka);
            stream.Close();
            this.tasma = new Tasma(sciezka);
            this.pozycjaOstatniejStrony = -1;
            this.pozycjaRoot = -1;
            unsafe
            {
                this.d = ((global.rozmiarStrony - sizeof(int)) / (9 + sizeof(int))) / 2;
            }
            this.bufor = new List<Tuple<List<Rekord>, List<int>, int>>();
            this.doWypisywania = new List<string>();
            this.h = 0;
            this.wolneAdresy = new List<int>();
        }

        private bool wyszukaj(int ident)
        {
            this.bufor.Clear();
            int pozycja = this.pozycjaRoot;
            for (;;)
            {
                if (pozycja == -1)
                {
                    return false;
                }
                else
                {
                    this.bufor.Add(this.tasma.wczytajStrone(pozycja));
                    int i = 0;
                    bool szukaj = true;
                    for (; i < this.bufor.Last().Item1.Count && szukaj; i++)
                    {
                        if (ident < this.bufor.Last().Item1[i].rekord.indeks)
                        {
                            pozycja = this.bufor.Last().Item2[i];
                            szukaj = false;
                            break;
                        }
                        else if (ident == this.bufor.Last().Item1[i].rekord.indeks)
                        {
                            return true;
                        }
                    }
                    if (i == this.bufor.Last().Item1.Count)
                    {
                        pozycja = this.bufor.Last().Item2.Last();
                    }
                }
            }
        }

        public void dodaj(Rekord input)
        {
            if (pozycjaRoot == -1)
            {
                this.pozycjaRoot = 0;
                this.pozycjaOstatniejStrony = 0;
                var strona = new Tuple<List<Rekord>, List<int>, int>(new List<Rekord>(), new List<int>(), pozycjaRoot);
                strona.Item1.Add(input);
                strona.Item2.Add(-1);
                strona.Item2.Add(-1);
                this.tasma.zapiszStrone(strona);
            }
            else
            {
                if (!this.wyszukaj(input.rekord.indeks))
                {
                    Rekord doWstawienia = input;
                    int wsk = -1;
                    for (;;)
                    {
                        if (this.bufor.Count == 0)
                        {
                            int pozycjaDoWstawienia;
                            if (this.wolneAdresy.Count == 0)
                            {
                                this.pozycjaOstatniejStrony += global.rozmiarStrony;
                                pozycjaDoWstawienia = pozycjaOstatniejStrony;
                            }
                            else
                            {
                                pozycjaDoWstawienia = this.wolneAdresy[0];
                                this.wolneAdresy.RemoveAt(0);
                            }
                            var nowyRoot = new Tuple<List<Rekord>, List<int>, int>(new List<Rekord>(), new List<int>(), pozycjaDoWstawienia);
                            nowyRoot.Item2.Add(this.pozycjaRoot);
                            this.pozycjaRoot = nowyRoot.Item3;
                            nowyRoot.Item2.Add(wsk);
                            nowyRoot.Item1.Add(input);
                            this.tasma.zapiszStrone(nowyRoot);
                            this.h++;
                            break;
                        }
                        else if (this.bufor.Last().Item1.Count < 2 * this.d)
                        {
                            int i = 0;
                            for (; i < this.bufor.Last().Item1.Count; i++)
                            {
                                if (input.rekord.indeks < this.bufor.Last().Item1[i].rekord.indeks)
                                {
                                    this.bufor.Last().Item1.Insert(i, input);
                                    this.bufor.Last().Item2.Insert(i + 1, wsk);
                                    this.tasma.zapiszStrone(this.bufor.Last());
                                    break;
                                }
                            }
                            if (i == this.bufor.Last().Item1.Count)
                            {
                                this.bufor.Last().Item1.Add(input);
                                this.bufor.Last().Item2.Add(wsk);
                                this.tasma.zapiszStrone(this.bufor.Last());
                            }
                            break;
                        }
                        else if (this.kompensacja(ref wsk, input))
                        {
                            global.licznikKom++;
                            break;
                        }
                        else
                        {
                            global.licznikSplit++;
                            this.rozdzielanie(ref input, ref wsk);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Element juz istnieje");
                }
            }
        }
        bool kompensacja(ref int wsk, Rekord input = null, bool wstawianie = true)
        {
            if (this.bufor.Count < 2)
            {
                return false;
            }
            else
            {
                int pozycjaBrata = this.bufor[this.bufor.Count - 2].Item2.FindIndex(x => x == this.bufor.Last().Item3) + 1;
                if (pozycjaBrata != -1 && pozycjaBrata < this.bufor[this.bufor.Count - 2].Item2.Count)
                {
                    var brat = this.tasma.wczytajStrone(this.bufor[this.bufor.Count - 2].Item2[pozycjaBrata]);
                    if (brat.Item1.Count < 2 * this.d && wstawianie)
                    {
                        int pozycjaRekorduPomiedzy = pozycjaBrata - 1;
                        int i = 0;
                        for (; i < this.bufor.Last().Item1.Count; i++)
                        {
                            if (input.rekord.indeks < this.bufor.Last().Item1[i].rekord.indeks)
                            {
                                this.bufor.Last().Item1.Insert(i, input);
                                this.bufor.Last().Item2.Insert(i + 1, wsk);
                                break;
                            }
                        }
                        if (i == this.bufor.Last().Item1.Count)
                        {
                            this.bufor.Last().Item1.Add(input);
                            this.bufor.Last().Item2.Add(wsk);
                        }

                        for (; brat.Item1.Count + 1 < this.bufor.Last().Item1.Count;)
                        {
                            brat.Item1.Insert(0, this.bufor[this.bufor.Count - 2].Item1[pozycjaRekorduPomiedzy]);
                            brat.Item2.Insert(0, this.bufor.Last().Item2.Last());
                            this.bufor[this.bufor.Count - 2].Item1[pozycjaRekorduPomiedzy] = this.bufor.Last().Item1.Last();
                            this.bufor.Last().Item1.Remove(this.bufor.Last().Item1.Last());
                            this.bufor.Last().Item2.Remove(this.bufor.Last().Item2.Last());
                        }
                        this.tasma.zapiszStrone(brat);
                        this.tasma.zapiszStrone(this.bufor.Last());
                        this.tasma.zapiszStrone(this.bufor[this.bufor.Count - 2]);
                        return true;
                    }
                    else if (brat.Item1.Count > this.d && !wstawianie)
                    {
                        int pozycjaRekorduPomiedzy = pozycjaBrata - 1;
                        for (; brat.Item1.Count > this.bufor.Last().Item1.Count + 1;)
                        {
                            this.bufor.Last().Item1.Add(this.bufor[this.bufor.Count - 2].Item1[pozycjaRekorduPomiedzy]);
                            this.bufor.Last().Item2.Add(brat.Item2[0]);
                            this.bufor[this.bufor.Count - 2].Item1[pozycjaRekorduPomiedzy] = brat.Item1[0];
                            brat.Item1.RemoveAt(0);
                            brat.Item2.RemoveAt(0);
                        }
                        this.tasma.zapiszStrone(brat);
                        this.tasma.zapiszStrone(this.bufor.Last());
                        this.tasma.zapiszStrone(this.bufor[this.bufor.Count - 2]);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    pozycjaBrata = this.bufor[this.bufor.Count - 2].Item2.FindIndex(x => x == this.bufor.Last().Item3) - 1;

                    var brat = this.tasma.wczytajStrone(this.bufor[this.bufor.Count - 2].Item2[pozycjaBrata]);
                    if (brat.Item1.Count < 2 * this.d && wstawianie)
                    {
                        int pozycjaRekorduPomiedzy = pozycjaBrata;
                        int i = 0;
                        for (; i < this.bufor.Last().Item1.Count; i++)
                        {
                            if (input.rekord.indeks < this.bufor.Last().Item1[i].rekord.indeks)
                            {
                                this.bufor.Last().Item1.Insert(i, input);
                                this.bufor.Last().Item2.Insert(i + 1, wsk);
                                break;
                            }
                        }
                        if (i == this.bufor.Last().Item1.Count)
                        {
                            this.bufor.Last().Item1.Add(input);
                            this.bufor.Last().Item2.Add(wsk);
                        }

                        for (; brat.Item1.Count + 1 < this.bufor.Last().Item1.Count;)
                        {
                            brat.Item1.Add(this.bufor[this.bufor.Count - 2].Item1[pozycjaRekorduPomiedzy]);
                            brat.Item2.Add(this.bufor.Last().Item2[0]);
                            this.bufor[this.bufor.Count - 2].Item1[pozycjaRekorduPomiedzy] = this.bufor.Last().Item1[0];
                            this.bufor.Last().Item1.RemoveAt(0);
                            this.bufor.Last().Item2.RemoveAt(0);
                        }
                        this.tasma.zapiszStrone(brat);
                        this.tasma.zapiszStrone(this.bufor.Last());
                        this.tasma.zapiszStrone(this.bufor[this.bufor.Count - 2]);
                        return true;
                    }
                    else if (brat.Item1.Count > this.d && !wstawianie)
                    {
                        int pozycjaRekorduPomiedzy = pozycjaBrata;
                        for (; brat.Item1.Count > this.bufor.Last().Item1.Count + 1;)
                        {
                            this.bufor.Last().Item1.Insert(0, this.bufor[this.bufor.Count - 2].Item1[pozycjaRekorduPomiedzy]);
                            this.bufor.Last().Item2.Insert(0, brat.Item2.Last());
                            this.bufor[this.bufor.Count - 2].Item1[pozycjaRekorduPomiedzy] = brat.Item1.Last();
                            brat.Item1.Remove(brat.Item1.Last());
                            brat.Item2.Remove(brat.Item2.Last());
                        }
                        this.tasma.zapiszStrone(brat);
                        this.tasma.zapiszStrone(this.bufor.Last());
                        this.tasma.zapiszStrone(this.bufor[this.bufor.Count - 2]);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        void rozdzielanie(ref Rekord input, ref int wsk)
        {
            int i = 0;
            for (; i < this.bufor.Last().Item1.Count; i++)
            {
                if (input.rekord.indeks < this.bufor.Last().Item1[i].rekord.indeks)
                {
                    this.bufor.Last().Item1.Insert(i, input);
                    this.bufor.Last().Item2.Insert(i + 1, wsk);
                    break;
                }
            }
            if (i == this.bufor.Last().Item1.Count)
            {
                this.bufor.Last().Item1.Add(input);
                this.bufor.Last().Item2.Add(wsk);
            }
            this.pozycjaOstatniejStrony += global.rozmiarStrony;
            var nowaStrona = new Tuple<List<Rekord>, List<int>, int>(new List<Rekord>(), new List<int>(), this.pozycjaOstatniejStrony);
            int srodek = this.bufor.Last().Item1.Count / 2;
            input = this.bufor.Last().Item1[srodek];
            wsk = this.pozycjaOstatniejStrony;
            nowaStrona.Item2.Add(this.bufor.Last().Item2[srodek + 1]);
            this.bufor.Last().Item1.RemoveAt(srodek);
            this.bufor.Last().Item2.RemoveAt(srodek + 1);
            for (; srodek < this.bufor.Last().Item1.Count;)
            {
                nowaStrona.Item1.Add(this.bufor.Last().Item1[srodek]);
                this.bufor.Last().Item1.RemoveAt(srodek);
                nowaStrona.Item2.Add(this.bufor.Last().Item2[srodek + 1]);
                this.bufor.Last().Item2.RemoveAt(srodek + 1);
            }
            this.tasma.zapiszStrone(nowaStrona);
            this.tasma.zapiszStrone(this.bufor.Last());
            this.bufor.Remove(this.bufor.Last());
        }

        public void usun(Rekord input)
        {
            int wsk = -1;
            if (this.wyszukaj(input.rekord.indeks))
            {
                for (;;)
                {
                    int pozycjaStrony = this.bufor.Last().Item1.FindIndex(x => x.rekord.indeks == input.rekord.indeks);
                    int wskaznikPoLewej = this.bufor.Last().Item2[pozycjaStrony];
                    int wskaznikPoPrawej = this.bufor.Last().Item2[pozycjaStrony + 1];
                    if (wskaznikPoLewej == -1 && wskaznikPoPrawej == -1)
                    {
                        this.bufor.Last().Item1.RemoveAt(pozycjaStrony);
                        this.bufor.Last().Item2.RemoveAt(pozycjaStrony + 1);
                        if (this.bufor.Last().Item1.Count >= this.d || this.bufor.Count == 1)
                        {
                            this.tasma.zapiszStrone(this.bufor.Last());
                            break;
                        }
                        else if (this.kompensacja(ref wsk, null, false))
                        {
                            break;
                        }
                        else
                        {
                            this.scalanie();
                            break;
                        }
                    }
                    else if (wskaznikPoLewej != -1)
                    {
                        this.bufor.Add(this.tasma.wczytajStrone(wskaznikPoLewej));
                        Rekord tmp = this.bufor[this.bufor.Count - 2].Item1[pozycjaStrony];
                        this.bufor[this.bufor.Count - 2].Item1[pozycjaStrony] = this.bufor.Last().Item1.Last();
                        this.bufor.Last().Item1[this.bufor.Last().Item1.Count - 1] = tmp;
                        this.tasma.zapiszStrone(this.bufor[this.bufor.Count - 2]);
                    }
                    else
                    {
                        this.bufor.Add(this.tasma.wczytajStrone(wskaznikPoPrawej));
                        Rekord tmp = this.bufor[this.bufor.Count - 2].Item1[pozycjaStrony];
                        this.bufor[this.bufor.Count - 2].Item1[pozycjaStrony] = this.bufor.Last().Item1[0];
                        this.bufor.Last().Item1[0] = tmp;
                        this.tasma.zapiszStrone(this.bufor[this.bufor.Count - 2]);
                    }
                }
            }
            else
            {
                Console.WriteLine("Element nie istnieje");
            }
        }

        bool scalanie()
        {
            if (this.bufor.Count > 2)
            {
                int pozycjaBrata = this.bufor[this.bufor.Count - 2].Item2.FindIndex(x => x == this.bufor.Last().Item3) + 1;
                if (pozycjaBrata != this.bufor[this.bufor.Count - 2].Item2.Count)
                {
                    var brat = this.tasma.wczytajStrone(this.bufor[this.bufor.Count - 2].Item2[pozycjaBrata]);
                    this.wolneAdresy.Add(this.bufor[this.bufor.Count - 2].Item2[pozycjaBrata]);
                    this.bufor.Last().Item1.Add(this.bufor[this.bufor.Count - 2].Item1[pozycjaBrata - 1]);
                    this.bufor[this.bufor.Count - 2].Item1.RemoveAt(pozycjaBrata - 1);
                    this.bufor[this.bufor.Count - 2].Item2.RemoveAt(pozycjaBrata);
                    this.bufor.Last().Item2.Add(brat.Item2[0]);
                    for (int i = 0; i < brat.Item1.Count; i++)
                    {
                        this.bufor.Last().Item1.Add(brat.Item1[i]);
                        this.bufor.Last().Item2.Add(brat.Item2[i + 1]);
                    }
                    this.tasma.zapiszStrone(this.bufor.Last());
                }
                else
                {
                    pozycjaBrata = this.bufor[this.bufor.Count - 2].Item2.FindIndex(x => x == this.bufor.Last().Item3) - 1;
                    var brat = this.tasma.wczytajStrone(this.bufor[this.bufor.Count - 2].Item2[pozycjaBrata]);
                    this.wolneAdresy.Add(this.bufor[this.bufor.Count - 2].Item2[pozycjaBrata]);
                    this.bufor.Last().Item1.Insert(0, this.bufor[this.bufor.Count - 2].Item1[pozycjaBrata]);
                    this.bufor[this.bufor.Count - 2].Item1.RemoveAt(pozycjaBrata);
                    this.bufor[this.bufor.Count - 2].Item2.RemoveAt(pozycjaBrata);
                    this.bufor.Last().Item2.Insert(0, brat.Item2[0]);
                    for (int i = brat.Item1.Count - 1; i >= 0; i--)
                    {
                        this.bufor.Last().Item1.Insert(0, brat.Item1[i]);
                        this.bufor.Last().Item2.Insert(0, brat.Item2[i + 1]);
                    }
                    this.tasma.zapiszStrone(this.bufor.Last());
                }
                if (this.bufor[this.bufor.Count - 2].Item1.Count > d)
                {
                    this.tasma.zapiszStrone(this.bufor[this.bufor.Count - 2]);
                    return true;
                }
                else
                {
                    int wsk = -1;
                    this.bufor.Remove(this.bufor.Last());
                    for (;;)
                    {
                        if (this.kompensacja(ref wsk, null, false))
                        {
                            global.licznikKom++;
                            break;
                        }
                        else if (this.scalanie())
                        {
                            global.licznikMerge++;
                            break;
                        }
                    }
                    return true;
                }
            }
            else
            {
                int pozycjaBrata = this.bufor[0].Item2[0];
                if (pozycjaBrata == this.bufor[1].Item3)
                {
                    pozycjaBrata = this.bufor[0].Item2[1];
                    var brat = this.tasma.wczytajStrone(pozycjaBrata);
                    this.bufor[1].Item1.Add(this.bufor[0].Item1[0]);
                    this.bufor[1].Item2.Add(brat.Item2[0]);
                    for (int i = 0; i < brat.Item1.Count; i++)
                    {
                        this.bufor[1].Item1.Add(brat.Item1[i]);
                        this.bufor[1].Item2.Add(brat.Item2[i + 1]);
                    }
                    this.wolneAdresy.Add(brat.Item3);
                }
                else
                {
                    var brat = this.tasma.wczytajStrone(pozycjaBrata);
                    this.bufor[1].Item1.Add(this.bufor[0].Item1[0]);
                    this.bufor[1].Item2.Insert(0, brat.Item2.Last());
                    for (int i = brat.Item1.Count - 1; i >= 0; i--)
                    {
                        this.bufor[1].Item1.Insert(0, brat.Item1[i]);
                        this.bufor[1].Item2.Insert(0, brat.Item2[i]);
                    }
                    this.wolneAdresy.Add(brat.Item3);
                }
                this.wolneAdresy.Add(this.pozycjaRoot);
                this.pozycjaRoot = this.bufor[1].Item3;
                this.tasma.zapiszStrone(this.bufor[1]);
                this.h--;
                return true;
            }
        }

        void wypiszStrone(int pozycja)
        {
            var strona = this.tasma.wczytajStrone(pozycja);
            Console.Write("*");
            for (int i = 0; i < strona.Item1.Count; i++)
                Console.Write("{0}*", strona.Item1[i].rekord.indeks);
            Console.Write(" ");
        }

        string stronaDoStringa(Tuple<List<Rekord>, List<int>, int> strona)
        {
            string output = "*";
            for (int i = 0; i < strona.Item1.Count; i++)
                output += strona.Item1[i].rekord.indeks.ToString() + "*";
            return output + " ";
        }

        void wypiszR(int pozycja = -1, int H = 0)
        {
            int pos;
            if (H == 0)
            {
                pos = this.pozycjaRoot;
            }
            else if (pozycja != -1)
            {
                pos = pozycja;
            }
            else
            {
                return;
            }
            if (this.doWypisywania.Count <= H)
            {
                for (int i = 0; i <= H - this.doWypisywania.Count; i++)
                {
                    this.doWypisywania.Add("");
                }
            }
            var strona = this.tasma.wczytajStrone(pos);
            this.doWypisywania[H] += this.stronaDoStringa(strona);
            for (int i = 0; i < strona.Item2.Count; i++)
            {
                this.wypiszR(strona.Item2[i], H + 1);
            }
        }
        public void wypisz()
        {
            this.doWypisywania.Clear();
            this.wypiszR(0, 0);
            for (int i = 0; i < this.doWypisywania.Count; i++)
            {
                Console.WriteLine(this.doWypisywania[i]);
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Drzewo tree = new Drzewo("drzewo.tree");
            int wypisuj = -1;
            for (;;)
            {
                string input = Console.ReadLine();
                if (input == "k")
                    break;
                else if (input == "d")
                {
                    string indeks = Console.ReadLine();
                    int inde;
                    Int32.TryParse(indeks, out inde);
                    tree.dodaj(new Rekord(inde, 1, 2, 3, 4, 5));
                }
                else if (input == "u")
                {
                    string indeks = Console.ReadLine();
                    int inde;
                    Int32.TryParse(indeks, out inde);
                    tree.usun(new Rekord(inde, 1, 2, 3, 4, 5));
                }
                else if (input == "w")
                {
                    wypisuj *= -1;
                }
                else
                    continue;
                if (wypisuj == 1)
                    tree.wypisz();
            }
        }
    }
}
