/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;

public class fuse_statfs_out : IDisposable, BufferWritable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal fuse_statfs_out(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(fuse_statfs_out obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~fuse_statfs_out() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      FuseWrapperPINVOKE.delete_fuse_statfs_out(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public fuse_kstatfs st {
    set {
      FuseWrapperPINVOKE.set_fuse_statfs_out_st(swigCPtr, fuse_kstatfs.getCPtr(value));
    } 
    get {
      IntPtr cPtr = FuseWrapperPINVOKE.get_fuse_statfs_out_st(swigCPtr);
      return (cPtr == IntPtr.Zero) ? null : new fuse_kstatfs(cPtr, false);
    } 
  }

  public int copyTo(FWBuffer buf) {
    return FuseWrapperPINVOKE.fuse_statfs_out_copyTo(swigCPtr, FWBuffer.getCPtr(buf));
  }

  public fuse_statfs_out() : this(FuseWrapperPINVOKE.new_fuse_statfs_out(), true) {
  }

}
