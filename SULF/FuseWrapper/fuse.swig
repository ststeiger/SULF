%module FuseWrapper

%{
#include "fuse-2.3.h"
#include "FWBuffer.h"
#include <unistd.h>
#include <errno.h>

/* forward definition to silence compiler */
static int FWBuffer_GetBytes( struct FWBuffer *, char *, int );
static int FWBuffer_AddBytes( struct FWBuffer *, char const *, int );

int close_fd( int fd )
{
    return close(fd);
}

#define READSTRUCT(BUF,TYPE) \
return FWBuffer_GetBytes( BUF, (char *)self, sizeof(struct TYPE) );

#define WRITESTRUCT(BUF,TYPE) \
return FWBuffer_AddBytes( BUF, (char const *)self, sizeof(struct TYPE) ); 

%}

#define _FILE_OFFSET_BITS 64

int close_fd( int fd );

/*
    change default for constants to have constants directly set in C# code,
    rather then having a wrapper to call into C to get the value..  The default
    (csconst(0)) ends up defining the values as statics, rather then constants.
*/
%csconst(1);

/*
    Add copyFrom functions to each input struct.  They copy data from the array
    passed in, and return the number of bytes used.
*/
%typemap(csinterfaces) struct fuse_in_header "IDisposable, BufferReadable"
%extend fuse_in_header {
    int copyFrom( struct FWBuffer *buf ) { READSTRUCT( buf, fuse_in_header ) }
    static int structSize() { return sizeof( struct fuse_in_header ); }
};

/*
    Extensions for writing out structs..
*/
%typemap(csinterfaces) struct fuse_out_header "IDisposable, BufferWritable"
%extend fuse_out_header {
    int copyTo( struct FWBuffer *buf ) { WRITESTRUCT( buf, fuse_out_header ) }
};

/* dirent is not constant size, it is handled by helper object FileBuffer, so
 it doesn't implement BufferWritable. */
%typemap(csinterfaces) struct fuse_dirent "IDisposable"

%typemap(csinterfaces) struct fuse_init_in_out 
    "IDisposable, BufferReadable, BufferWritable"
%extend fuse_init_in_out {
    int copyFrom( struct FWBuffer *buf ) { READSTRUCT( buf, fuse_init_in_out ) }
    int copyTo( struct FWBuffer *buf ) { WRITESTRUCT( buf, fuse_init_in_out ) }
};

%include "fuse-typemap.i"
%include "fuse-extract.i"
%include "fuse-types.i"
%include "FWBuffer.h"

