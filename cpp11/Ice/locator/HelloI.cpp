// **********************************************************************
//
// Copyright (c) 2003-2015 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

#include <IceUtil/IceUtil.h>
#include <Ice/Ice.h>
#include <HelloI.h>

using namespace std;

void
HelloI::sayHello(const Ice::Current&)
{
    cout << "Hello World!" << endl;
}

void
HelloI::shutdown(const Ice::Current& c)
{
    cout << "Shutting down..." << endl;

    //
    // Unregister from the Locator registry
    //
    auto communicator = c.adapter->getCommunicator();
    communicator->getDefaultLocator()->getRegistry()->setAdapterDirectProxy("Hello", nullptr);
    communicator->shutdown();
}
