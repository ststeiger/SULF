/*****************************************************************************
 * Author:   Valient Gough <vgough@pobox.com>
 *
 *****************************************************************************
 * Copyright (c) 2004, Valient Gough
 *
 * This library is free software; you can distribute it and/or modify it under
 * the terms of the GNU General Public License (GPL), as published by the Free
 * Software Foundation; either version 2 of the License, or (at your option)
 * any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GPL in the file COPYING for more
 * details.
 */

#ifndef _FWBuffer_incl_
#define _FWBuffer_incl_

#ifndef SWIG
#include <string.h>
#include <inttypes.h>
#endif

#ifdef SWIG
// add BufferWritable interface
%typemap(csinterfaces) struct FWBuffer "IDisposable, BufferWritable"
%typemap(csinterfaces) struct FileBuffer "IDisposable, BufferWritable"

%apply int { __s32 };
%apply unsigned int { __u32 };
#if __WORDSIZE == 64
%apply unsigned long { __u64 };
#else
%apply unsigned long long { __u64 };
#endif

#endif

#ifdef IN_DOXYGEN
/* This is what the class looks like from C#, so this is how we'll document
 it..  None of this is real though -- just for Doxygen! */

/** @class BufferReadable
  @brief Simple interface for objects that can be read from a FWBuffer

  @relates FWBuffer
*/
class BufferReadable
{
public:
    int copyFrom( FWBuffer buf );
};

/** @class BufferWritable
  @brief Simple interface for objects that can be written to a FWBuffer

  @relates FWBuffer
*/
class BufferWritable
{
public:
    int copyTo( FWBuffer buf );
};

/** @class FWBuffer
  @brief A C-managed buffer with convenience methods.
  
  FWBuffer is a SWIG produced wrapper for a buffer in C-space (unmanaged code
  in .net terms).   Convenience functions are provided for reading / writing to
  the buffer, and adding and pulling out data.

  All read and write functions operation based on the current offset.  The
  offset can be reset to 0 using Reset().

  In addition, all read and write functions will increment the offset.  Read
  functions will also increment the initialized counter.
*/
class FWBuffer : public BufferWritable
{
public:
    int initialized; /** number of bytes initialized*/
    int length;      /** real length of array */
    int offset;      /** current copy offset */
    char *data;

    /// Allocate a buffer with the specified initialize size.
    FWBuffer(int size);
    ~FWBuffer();
    /// Rewind buffer.  Sets current offset to 0.
    void Rewind();
    /** Reset to uninitialized. Sets current offset to 0, and resets
      initialized to 0. 
     */
    void Reset();
    /** Make sure there are at least @a freeBytes of space starting at the
     * current offset.  Reallocates the buffer if necessary. */
    int EnsureFreeSpace(int freeBytes);
    /** Read into the buffer at the current offset.  The max read size depends
     * on the remaining space in the buffer.
    */
    int Read(int fd);
    /** Read into the buffer at the current offset.  Space will be guarenteed
     * for @a count bytes.  The offset specifies where to read the data from,
     * not where the data will go in the buffer.
    */
    int Read(int fd, size_t count, __u64 offset);
    /** Write data from the buffer at the current offset.  Count bytes will be
     * written to the specified offset of the file.
    */
    int Write(int fd, size_t count, __u64 offset);
    /** Write the entire buffer (starting at the current offset) into the file,
     * as its current offset.
    */
    int Write(int fd);
    /** Return a string from the buffer.  The returned string is allocated with
     * strdup, and becomes the responsibility of the caller.
    */
    const char *GetString();
    int GetBytes( char *data, int len )
    int GetByte()

    /** Add bytes to the buffer at the current offset.  */
    int AddBytes( const char *data, int len );
    /** Add a string to the buffer (including the terminating null) at the
     * current offset.  */
    int AddString(const char *str);
    /** Add a portion of a string.
    */
    int AddSubString(const char *str, int offset, int len);
    /** Add another buffer contents to the buffer at the current offset.  */
    int AddBuffer( struct FWBuffer *arg );
    /** Part of BufferWritable protocol - copies contents to another buffer. 
      bufA.copyTo( bufB )  is the same as   bufB.AddBuffer( bufA )
    */
    int copyTo( struct FWBuffer *outBuf );
};
#else
struct FWBuffer
{
    int offset;      /* current copy offset */
#  ifdef SWIG
    %immutable;
    %nodefault;
#  endif
    int initialized; /* number of bytes initialized*/
    int length;      /* real length of array */
    char *data;
};
#endif

#ifdef SWIG
%typemap(csinterfaces) struct FWBuffer "IDisposable, BufferWritable"

%contract FWBuffer::Read(int fd) {
require:
    fd >= 0;
}
%contract FWBuffer::Write(int fd) {
require:
    fd >= 0;
}

%extend FWBuffer {
    FWBuffer(int size)
    {
	struct FWBuffer *buf = malloc(sizeof(struct FWBuffer));
	buf->data = malloc(size);
	buf->initialized = 0;
	buf->length = size;
	buf->offset = 0;
	return buf;
    }
    ~FWBuffer()
    {
	self->initialized = 0;
	self->length = 0;
	self->offset = 0;
	free(self->data);
	self->data = 0;
	free(self);
	self = 0;
    }

    void Rewind()
    {
	self->offset = 0;
    }

    void Reset()
    {
	self->initialized = 0;
	self->offset = 0;
	memset(self->data, 0, self->length);
    }
    
    /* ensure the given amount of free space */
    int EnsureFreeSpace(int freeBytes)
    {
	if( freeBytes + self->offset > self->length )
	{
	    int newSize = 2 * self->length;
	    void *ptr;
	    if(freeBytes + self->offset > newSize)
		newSize = freeBytes + self->offset;
	    ptr = realloc( self->data, newSize );
	    if(ptr == NULL)
		return -ENOMEM;
	    self->data = ptr;
	    self->length = newSize;
	}

    	return 0;
    }


    int Read(int fd)
    {
	int result = read( fd, self->data + self->offset, 
		self->length - self->offset );
	if(result < 0)
	    result = -errno;
	else
	    self->initialized += result;

	return result;
    }
    
    int Read(int fd, size_t count, __u64 offset)
    {
	int result;
	FWBuffer_EnsureFreeSpace( self, count );
	result = pread( fd, self->data + self->offset, count, offset );
	if(result < 0)
	    result = -errno;
	else
	    self->initialized += result;

	return result;
    }
    
    int Write(int fd, size_t count, __u64 offset)
    {
	int result;
	if(self->offset + count > self->initialized)
	    return -EIO;

	result = pwrite( fd, self->data + self->offset, count, offset );
	if(result < 0)
	    result = -errno;
	else
	    self->offset += result;

	return result;
    }

    /*
	Write from the start of the buffer, every initialized byte.
    */
    int Write(int fd)
    {
	int result = write( fd, self->data + self->offset, 
		self->initialized - self->offset );
	if(result < 0)
	    result = -errno;
	else
	    self->offset += result;

	return result;
    }

    const char *GetString()
    {
	const char *result = (char*)self->data + self->offset;
	int len = strlen(result);
	if(len + self->offset > self->initialized )
	    return NULL;
	else
	{
	    self->offset += len + 1;
	    return strdup(result);
	}
    }

    int GetBytes( char *data, int len )
    {
	if( self->initialized - self->offset >= len )
	{
	    memcpy( data, self->data + self->offset, len );
	    self->offset += len;
	    return len;
	} else
	    return -1;
    }
    
    int GetByte()
    {
	if( self->initialized > self->offset )
	{
	    int res = (int)*((unsigned char*)self->data + self->offset);
	    ++self->offset;
	    return res;
	} else
	    return -1;
    }

    int AddBytes( const char *data, int len )
    {
	int res = FWBuffer_EnsureFreeSpace( self, len );
	if(res == 0)
	{
	    memcpy( self->data + self->offset, data, len );
	    self->offset += len;
	    if(self->initialized < self->offset)
		self->initialized = self->offset;
	    res = len;
	}
	return res;
    }
    
    int AddChar( int len, char value )
    {
	int res = FWBuffer_EnsureFreeSpace( self, len );
	if(res == 0)
	{
	    memset( self->data + self->offset, value, len );
	    self->offset += len;
	    if(self->initialized < self->offset)
		self->initialized = self->offset;
	}
	return res;
    }

    int AddString(const char *str)
    {
	int len = strlen( str ) + 1;
	return FWBuffer_AddBytes( self, str, len );
    }
    
    int AddSubString(const char *str, int offset, int len)
    {
	return FWBuffer_AddBytes( self, str+offset, len );
    }

    int AddBuffer( struct FWBuffer *arg )
    {
	return FWBuffer_AddBytes( self, 
		arg->data + arg->offset, arg->initialized - arg->offset);
    }

    /* methods for BufferWritable interface */
    int copyTo( struct FWBuffer *outBuf )
    {
	return FWBuffer_AddBytes( outBuf, 
		self->data + self->offset, self->initialized - self->offset);
    }
}
#endif


#ifdef IN_DOXYGEN
/* This is what the class looks like from C#, so this is how we'll document
 it..  None of this is real though -- just for Doxygen! */
/** @class FileBuffer
  @brief Helper class for storing readdir information.

  The FileBuffer.addFile method is designed to be compatible with the
  ReadDirCallback delegate, so it can be used directly for filling information
  from a readdir call.
*/
class FileBuffer : public BufferWritable
{
public:
    FileBuffer();
    ~FileBuffer();

    /** add the filename with given type, node id and [next] offset.
    */
    int addFile( const char *name, __u64 nodeId, __u32 type, __u64 off );

    int copyTo( struct FWBuffer *outBuf );

    // set to point to a buffer before using..
    FWBuffer *buf;
    int error;
};
#else
struct FileBuffer
{
#  ifdef SWIG
    %immutable;
    %nodefault;
#  endif
    struct FWBuffer *buf;
    int error;
};
#endif

#ifdef SWIG
%extend FileBuffer {
    FileBuffer( struct FWBuffer *buffer )
    {
	struct FileBuffer *buf = malloc(sizeof(struct FileBuffer));
	buf->buf = buffer;
	buf->error = 0;
	return buf;
    }
    ~FileBuffer()
    {
	free( self );
    }
    int addFile( const char *name, __u64 nodeId, __u32 type, __u64 off )
    {
	// make sure buffer has enough space
	unsigned namelen = strlen( name );
	unsigned entlen;
	unsigned entsize;
	unsigned newlen;
	unsigned padlen;
	struct fuse_dirent dirent;

	if(namelen > FUSE_NAME_MAX)
	    namelen = FUSE_NAME_MAX;
	else if (!namelen)
	{
	    self->error = -EIO;
	    return 1;
	}

	entlen = FUSE_NAME_OFFSET + namelen;
	entsize = FUSE_DIRENT_ALIGN( entlen );
	padlen = entsize - entlen;
	newlen = self->buf->offset + entsize;
	//if(off && newlen > self->reqLen ) return 1;

	// TODO: allow inode number passthru
	dirent.ino = -1;
	dirent.off = off ? off : newlen;
	dirent.namelen = namelen;
	dirent.type = type;

	FWBuffer_AddBytes( self->buf, (char*)&dirent, 
		sizeof(struct fuse_dirent));
	FWBuffer_AddBytes( self->buf, name, namelen );
	if(padlen)
	    FWBuffer_AddChar( self->buf, padlen, '\0' );

	return 0;
    }
    
    int copyTo( struct FWBuffer *outBuf )
    {
	int res;
	int off = self->buf->offset;
	FWBuffer_Rewind( self->buf );
	res = FWBuffer_copyTo( self->buf, outBuf );
	self->buf->offset = off;
	return res;
    }
}
#endif /* SWIG */


#endif


