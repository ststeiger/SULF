/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;

public class fuse_dirent : IDisposable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal fuse_dirent(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(fuse_dirent obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~fuse_dirent() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      FuseWrapperPINVOKE.delete_fuse_dirent(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public ulong ino {
    set {
      FuseWrapperPINVOKE.set_fuse_dirent_ino(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_dirent_ino(swigCPtr);
    } 
  }

  public ulong off {
    set {
      FuseWrapperPINVOKE.set_fuse_dirent_off(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_dirent_off(swigCPtr);
    } 
  }

  public uint namelen {
    set {
      FuseWrapperPINVOKE.set_fuse_dirent_namelen(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_dirent_namelen(swigCPtr);
    } 
  }

  public uint type {
    set {
      FuseWrapperPINVOKE.set_fuse_dirent_type(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_dirent_type(swigCPtr);
    } 
  }

  public string name {
    set {
      FuseWrapperPINVOKE.set_fuse_dirent_name(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_dirent_name(swigCPtr);
    } 
  }

  public fuse_dirent() : this(FuseWrapperPINVOKE.new_fuse_dirent(), true) {
  }

}
