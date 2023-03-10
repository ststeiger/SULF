/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;

public class fuse_init_in_out : IDisposable, BufferWritable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal fuse_init_in_out(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(fuse_init_in_out obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~fuse_init_in_out() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      FuseWrapperPINVOKE.delete_fuse_init_in_out(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public uint major {
    set {
      FuseWrapperPINVOKE.set_fuse_init_in_out_major(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_init_in_out_major(swigCPtr);
    } 
  }

  public uint minor {
    set {
      FuseWrapperPINVOKE.set_fuse_init_in_out_minor(swigCPtr, value);
    } 
    get {
      return FuseWrapperPINVOKE.get_fuse_init_in_out_minor(swigCPtr);
    } 
  }

  public int copyFrom(FWBuffer buf) {
    return FuseWrapperPINVOKE.fuse_init_in_out_copyFrom(swigCPtr, FWBuffer.getCPtr(buf));
  }

  public int copyTo(FWBuffer buf) {
    return FuseWrapperPINVOKE.fuse_init_in_out_copyTo(swigCPtr, FWBuffer.getCPtr(buf));
  }

  public fuse_init_in_out() : this(FuseWrapperPINVOKE.new_fuse_init_in_out(), true) {
  }

}
