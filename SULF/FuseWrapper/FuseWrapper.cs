/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;

public class FuseWrapper {
  public static int close_fd(int fd) {
    return FuseWrapperPINVOKE.close_fd(fd);
  }

  public static readonly int _FILE_OFFSET_BITS = FuseWrapperPINVOKE.get__FILE_OFFSET_BITS();
  public const int FUSE_KERNEL_VERSION = 6;
  public const int FUSE_KERNEL_MINOR_VERSION = 1;
  public const int FUSE_ROOT_ID = 1;
  public const int FUSE_MAJOR = 10;
  public const int FUSE_MINOR = 229;
  public const int FATTR_MODE = (1<<0);
  public const int FATTR_UID = (1<<1);
  public const int FATTR_GID = (1<<2);
  public const int FATTR_SIZE = (1<<3);
  public const int FATTR_ATIME = (1<<4);
  public const int FATTR_MTIME = (1<<5);
  public const int FATTR_CTIME = (1<<6);
  public const int FUSE_MAX_IN = 8192;
  public const int FUSE_NAME_MAX = 1024;
  public const int FUSE_SYMLINK_MAX = 4096;
  public const int FUSE_XATTR_SIZE_MAX = 4096;
}