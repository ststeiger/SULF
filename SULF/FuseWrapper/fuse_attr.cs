/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;

public class fuse_attr : IDisposable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal fuse_attr(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(fuse_attr obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~fuse_attr() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      FuseWrapperPINVOKE.delete_fuse_attr(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public ulong ino {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_ino(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_ino(swigCPtr);
    } 
  }

  public ulong size {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_size(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_size(swigCPtr);
    } 
  }

  public ulong blocks {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_blocks(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_blocks(swigCPtr);
    } 
  }

  public ulong atime {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_atime(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_atime(swigCPtr);
    } 
  }

  public ulong mtime {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_mtime(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_mtime(swigCPtr);
    } 
  }

  public ulong ctime {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_ctime(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_ctime(swigCPtr);
    } 
  }

  public uint atimensec {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_atimensec(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_atimensec(swigCPtr);
    } 
  }

  public uint mtimensec {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_mtimensec(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_mtimensec(swigCPtr);
    } 
  }

  public uint ctimensec {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_ctimensec(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_ctimensec(swigCPtr);
    } 
  }

  public uint mode {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_mode(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_mode(swigCPtr);
    } 
  }

  public uint nlink {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_nlink(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_nlink(swigCPtr);
    } 
  }

  public uint uid {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_uid(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_uid(swigCPtr);
    } 
  }

  public uint gid {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_gid(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_gid(swigCPtr);
    } 
  }

  public uint rdev {
    set {
      FuseWrapperPINVOKE.set_fuse_attr_rdev(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_attr_rdev(swigCPtr);
    } 
  }

  public fuse_attr() : this(FuseWrapperPINVOKE.new_fuse_attr(), true) {
  }

}
