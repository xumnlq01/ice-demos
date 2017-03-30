// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

using Demo;
using System;
using System.Reflection;

[assembly: CLSCompliant(true)]

[assembly: AssemblyTitle("IceHelloClient")]
[assembly: AssemblyDescription("Ice hello demo client")]
[assembly: AssemblyCompany("ZeroC, Inc.")]

public class Client
{
    public class App : Ice.Application
    {
        private static void menu()
        {
            Console.Write(
                "usage:\n" +
                "t: send greeting as twoway\n" +
                "o: send greeting as oneway\n" +
                "O: send greeting as batch oneway\n" +
                "d: send greeting as datagram\n" +
                "D: send greeting as batch datagram\n" +
                "f: flush all batch requests\n" +
                "T: set a timeout\n" +
                "P: set a server delay\n" +
                "S: switch secure mode on/off\n" +
                "s: shutdown server\n" +
                "x: exit\n" +
                "?: help\n");
        }

        public override int run(string[] args)
        {
            if(args.Length > 0)
            {
                Console.Error.WriteLine(appName() + ": too many arguments");
                return 1;
            }

            HelloPrx twoway = HelloPrxHelper.checkedCast(
                communicator().propertyToProxy("Hello.Proxy").ice_twoway().ice_secure(false));
            if(twoway == null)
            {
                Console.Error.WriteLine("invalid proxy");
                return 1;
            }
            HelloPrx oneway = (HelloPrx)twoway.ice_oneway();
            HelloPrx batchOneway = (HelloPrx)twoway.ice_batchOneway();
            HelloPrx datagram = (HelloPrx)twoway.ice_datagram();
            HelloPrx batchDatagram =(HelloPrx)twoway.ice_batchDatagram();

            bool secure = false;
            int timeout = -1;
            int delay = 0;

            menu();

            string line = null;
            do
            {
                try
                {
                    Console.Out.Write("==> ");
                    Console.Out.Flush();
                    line = Console.In.ReadLine();
                    if(line == null)
                    {
                        break;
                    }
                    if(line.Equals("t"))
                    {
                        twoway.sayHello(delay);
                    }
                    else if(line.Equals("o"))
                    {
                        oneway.sayHello(delay);
                    }
                    else if(line.Equals("O"))
                    {
                        batchOneway.sayHello(delay);
                    }
                    else if(line.Equals("d"))
                    {
                        if(secure)
                        {
                            Console.WriteLine("secure datagrams are not supported");
                        }
                        else
                        {
                            datagram.sayHello(delay);
                        }
                    }
                    else if(line.Equals("D"))
                    {
                        if(secure)
                        {
                            Console.WriteLine("secure datagrams are not supported");
                        }
                        else
                        {
                            batchDatagram.sayHello(delay);
                        }
                    }
                    else if(line.Equals("f"))
                    {
                        batchOneway.ice_flushBatchRequests();
                        batchDatagram.ice_flushBatchRequests();
                    }
                    else if(line.Equals("T"))
                    {
                        if(timeout == -1)
                        {
                            timeout = 2000;
                        }
                        else
                        {
                            timeout = -1;
                        }

                        twoway = (HelloPrx)twoway.ice_invocationTimeout(timeout);
                        oneway = (HelloPrx)oneway.ice_invocationTimeout(timeout);
                        batchOneway = (HelloPrx)batchOneway.ice_invocationTimeout(timeout);

                        if(timeout == -1)
                        {
                            Console.WriteLine("timeout is now switched off");
                        }
                        else
                        {
                            Console.WriteLine("timeout is now set to 2000ms");
                        }
                    }
                    else if(line.Equals("P"))
                    {
                        if(delay == 0)
                        {
                            delay = 2500;
                        }
                        else
                        {
                            delay = 0;
                        }

                        if(delay == 0)
                        {
                            Console.WriteLine("server delay is now deactivated");
                        }
                        else
                        {
                            Console.WriteLine("server delay is now set to 2500ms");
                        }
                    }
                    else if(line.Equals("S"))
                    {
                        secure = !secure;

                        twoway = (HelloPrx)twoway.ice_secure(secure);
                        oneway = (HelloPrx)oneway.ice_secure(secure);
                        batchOneway = (HelloPrx)batchOneway.ice_secure(secure);
                        datagram = (HelloPrx)datagram.ice_secure(secure);
                        batchDatagram = (HelloPrx)batchDatagram.ice_secure(secure);

                        if(secure)
                        {
                            Console.WriteLine("secure mode is now on");
                        }
                        else
                        {
                            Console.WriteLine("secure mode is now off");
                        }
                    }
                    else if(line.Equals("s"))
                    {
                        twoway.shutdown();
                    }
                    else if(line.Equals("x"))
                    {
                        // Nothing to do
                    }
                    else if(line.Equals("?"))
                    {
                        menu();
                    }
                    else
                    {
                        Console.WriteLine("unknown command `" + line + "'");
                        menu();
                    }
                }
                catch(System.Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
            while (!line.Equals("x"));

            return 0;
        }
    }

    public static int Main(string[] args)
    {
        App app = new App();
        return app.main(args, "config.client");
    }
}
