/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;

public class fuse_forget_in : IDisposable, BufferReadable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal fuse_forget_in(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(fuse_forget_in obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~fuse_forget_in() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      FuseWrapperPINVOKE.delete_fuse_forget_in(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public ulong version {
    get {
      return FuseWrapperPINVOKE.get_fuse_forget_in_version(swigCPtr);
    } 
  }

  public int copyFrom(FWBuffer buf) {
    return FuseWrapperPINVOKE.fuse_forget_in_copyFrom(swigCPtr, FWBuffer.getCPtr(buf));
  }

  public fuse_forget_in() : this(FuseWrapperPINVOKE.new_fuse_forget_in(), true) {
  }

}
