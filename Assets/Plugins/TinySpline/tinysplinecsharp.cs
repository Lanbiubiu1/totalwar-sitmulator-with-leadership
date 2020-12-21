//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace TinySpline {

public class tinysplinecsharp {
  public static uint ts_bspline_degree(SWIGTYPE_p_tsBSpline spline) {
    uint ret = tinysplinecsharpPINVOKE.ts_bspline_degree(SWIGTYPE_p_tsBSpline.getCPtr(spline));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_set_degree(SWIGTYPE_p_tsBSpline spline, uint deg, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_set_degree(SWIGTYPE_p_tsBSpline.getCPtr(spline), deg, SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_bspline_order(SWIGTYPE_p_tsBSpline spline) {
    uint ret = tinysplinecsharpPINVOKE.ts_bspline_order(SWIGTYPE_p_tsBSpline.getCPtr(spline));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_set_order(SWIGTYPE_p_tsBSpline spline, uint order, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_set_order(SWIGTYPE_p_tsBSpline.getCPtr(spline), order, SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_bspline_dimension(SWIGTYPE_p_tsBSpline spline) {
    uint ret = tinysplinecsharpPINVOKE.ts_bspline_dimension(SWIGTYPE_p_tsBSpline.getCPtr(spline));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_set_dimension(SWIGTYPE_p_tsBSpline spline, uint dim, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_set_dimension(SWIGTYPE_p_tsBSpline.getCPtr(spline), dim, SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_bspline_len_control_points(SWIGTYPE_p_tsBSpline spline) {
    uint ret = tinysplinecsharpPINVOKE.ts_bspline_len_control_points(SWIGTYPE_p_tsBSpline.getCPtr(spline));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_bspline_num_control_points(SWIGTYPE_p_tsBSpline spline) {
    uint ret = tinysplinecsharpPINVOKE.ts_bspline_num_control_points(SWIGTYPE_p_tsBSpline.getCPtr(spline));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_bspline_sof_control_points(SWIGTYPE_p_tsBSpline spline) {
    uint ret = tinysplinecsharpPINVOKE.ts_bspline_sof_control_points(SWIGTYPE_p_tsBSpline.getCPtr(spline));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_control_points(SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_p_double ctrlp, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_control_points(SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_p_double.getCPtr(ctrlp), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_control_point_at(SWIGTYPE_p_tsBSpline spline, uint index, SWIGTYPE_p_p_double ctrlp, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_control_point_at(SWIGTYPE_p_tsBSpline.getCPtr(spline), index, SWIGTYPE_p_p_double.getCPtr(ctrlp), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_set_control_points(SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_double ctrlp, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_set_control_points(SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_double.getCPtr(ctrlp), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_set_control_point_at(SWIGTYPE_p_tsBSpline spline, uint index, SWIGTYPE_p_double ctrlp, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_set_control_point_at(SWIGTYPE_p_tsBSpline.getCPtr(spline), index, SWIGTYPE_p_double.getCPtr(ctrlp), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_bspline_num_knots(SWIGTYPE_p_tsBSpline spline) {
    uint ret = tinysplinecsharpPINVOKE.ts_bspline_num_knots(SWIGTYPE_p_tsBSpline.getCPtr(spline));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_bspline_sof_knots(SWIGTYPE_p_tsBSpline spline) {
    uint ret = tinysplinecsharpPINVOKE.ts_bspline_sof_knots(SWIGTYPE_p_tsBSpline.getCPtr(spline));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_knots(SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_p_double knots, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_knots(SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_p_double.getCPtr(knots), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_knot_at(SWIGTYPE_p_tsBSpline spline, uint index, SWIGTYPE_p_double knot, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_knot_at(SWIGTYPE_p_tsBSpline.getCPtr(spline), index, SWIGTYPE_p_double.getCPtr(knot), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_set_knots(SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_double knots, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_set_knots(SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_double.getCPtr(knots), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_set_knot_at(SWIGTYPE_p_tsBSpline spline, uint index, double knot, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_set_knot_at(SWIGTYPE_p_tsBSpline.getCPtr(spline), index, knot, SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static double ts_deboornet_knot(SWIGTYPE_p_tsDeBoorNet net) {
    double ret = tinysplinecsharpPINVOKE.ts_deboornet_knot(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_deboornet_index(SWIGTYPE_p_tsDeBoorNet net) {
    uint ret = tinysplinecsharpPINVOKE.ts_deboornet_index(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_deboornet_multiplicity(SWIGTYPE_p_tsDeBoorNet net) {
    uint ret = tinysplinecsharpPINVOKE.ts_deboornet_multiplicity(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_deboornet_num_insertions(SWIGTYPE_p_tsDeBoorNet net) {
    uint ret = tinysplinecsharpPINVOKE.ts_deboornet_num_insertions(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_deboornet_dimension(SWIGTYPE_p_tsDeBoorNet net) {
    uint ret = tinysplinecsharpPINVOKE.ts_deboornet_dimension(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_deboornet_len_points(SWIGTYPE_p_tsDeBoorNet net) {
    uint ret = tinysplinecsharpPINVOKE.ts_deboornet_len_points(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_deboornet_num_points(SWIGTYPE_p_tsDeBoorNet net) {
    uint ret = tinysplinecsharpPINVOKE.ts_deboornet_num_points(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_deboornet_sof_points(SWIGTYPE_p_tsDeBoorNet net) {
    uint ret = tinysplinecsharpPINVOKE.ts_deboornet_sof_points(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_deboornet_points(SWIGTYPE_p_tsDeBoorNet net, SWIGTYPE_p_p_double points, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_deboornet_points(SWIGTYPE_p_tsDeBoorNet.getCPtr(net), SWIGTYPE_p_p_double.getCPtr(points), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_deboornet_len_result(SWIGTYPE_p_tsDeBoorNet net) {
    uint ret = tinysplinecsharpPINVOKE.ts_deboornet_len_result(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_deboornet_num_result(SWIGTYPE_p_tsDeBoorNet net) {
    uint ret = tinysplinecsharpPINVOKE.ts_deboornet_num_result(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint ts_deboornet_sof_result(SWIGTYPE_p_tsDeBoorNet net) {
    uint ret = tinysplinecsharpPINVOKE.ts_deboornet_sof_result(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_deboornet_result(SWIGTYPE_p_tsDeBoorNet net, SWIGTYPE_p_p_double result, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_deboornet_result(SWIGTYPE_p_tsDeBoorNet.getCPtr(net), SWIGTYPE_p_p_double.getCPtr(result), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_p_tsBSpline ts_bspline_init() {
    SWIGTYPE_p_tsBSpline ret = new SWIGTYPE_p_tsBSpline(tinysplinecsharpPINVOKE.ts_bspline_init(), true);
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_new(uint num_control_points, uint dimension, uint degree, BSplineType type, SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_new(num_control_points, dimension, degree, (int)type, SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_copy(SWIGTYPE_p_tsBSpline src, SWIGTYPE_p_tsBSpline dest, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_copy(SWIGTYPE_p_tsBSpline.getCPtr(src), SWIGTYPE_p_tsBSpline.getCPtr(dest), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static void ts_bspline_move(SWIGTYPE_p_tsBSpline src, SWIGTYPE_p_tsBSpline dest) {
    tinysplinecsharpPINVOKE.ts_bspline_move(SWIGTYPE_p_tsBSpline.getCPtr(src), SWIGTYPE_p_tsBSpline.getCPtr(dest));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
  }

  public static void ts_bspline_free(SWIGTYPE_p_tsBSpline spline) {
    tinysplinecsharpPINVOKE.ts_bspline_free(SWIGTYPE_p_tsBSpline.getCPtr(spline));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
  }

  public static SWIGTYPE_p_tsDeBoorNet ts_deboornet_init() {
    SWIGTYPE_p_tsDeBoorNet ret = new SWIGTYPE_p_tsDeBoorNet(tinysplinecsharpPINVOKE.ts_deboornet_init(), true);
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_deboornet_copy(SWIGTYPE_p_tsDeBoorNet src, SWIGTYPE_p_tsDeBoorNet dest, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_deboornet_copy(SWIGTYPE_p_tsDeBoorNet.getCPtr(src), SWIGTYPE_p_tsDeBoorNet.getCPtr(dest), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static void ts_deboornet_move(SWIGTYPE_p_tsDeBoorNet src, SWIGTYPE_p_tsDeBoorNet dest) {
    tinysplinecsharpPINVOKE.ts_deboornet_move(SWIGTYPE_p_tsDeBoorNet.getCPtr(src), SWIGTYPE_p_tsDeBoorNet.getCPtr(dest));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
  }

  public static void ts_deboornet_free(SWIGTYPE_p_tsDeBoorNet net) {
    tinysplinecsharpPINVOKE.ts_deboornet_free(SWIGTYPE_p_tsDeBoorNet.getCPtr(net));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
  }

  public static SWIGTYPE_tsError ts_bspline_interpolate_cubic_natural(SWIGTYPE_p_double points, uint num_points, uint dimension, SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_interpolate_cubic_natural(SWIGTYPE_p_double.getCPtr(points), num_points, dimension, SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_interpolate_catmull_rom(SWIGTYPE_p_double points, uint num_points, uint dimension, double alpha, SWIGTYPE_p_double first, SWIGTYPE_p_double last, double epsilon, SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_interpolate_catmull_rom(SWIGTYPE_p_double.getCPtr(points), num_points, dimension, alpha, SWIGTYPE_p_double.getCPtr(first), SWIGTYPE_p_double.getCPtr(last), epsilon, SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_eval(SWIGTYPE_p_tsBSpline spline, double u, SWIGTYPE_p_tsDeBoorNet net, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_eval(SWIGTYPE_p_tsBSpline.getCPtr(spline), u, SWIGTYPE_p_tsDeBoorNet.getCPtr(net), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_eval_all(SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_double us, uint num, SWIGTYPE_p_p_double points, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_eval_all(SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_double.getCPtr(us), num, SWIGTYPE_p_p_double.getCPtr(points), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_sample(SWIGTYPE_p_tsBSpline spline, uint num, SWIGTYPE_p_p_double points, SWIGTYPE_p_size_t actual_num, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_sample(SWIGTYPE_p_tsBSpline.getCPtr(spline), num, SWIGTYPE_p_p_double.getCPtr(points), SWIGTYPE_p_size_t.getCPtr(actual_num), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_bisect(SWIGTYPE_p_tsBSpline spline, double value, double epsilon, int persnickety, uint index, int ascending, uint max_iter, SWIGTYPE_p_tsDeBoorNet net, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_bisect(SWIGTYPE_p_tsBSpline.getCPtr(spline), value, epsilon, persnickety, index, ascending, max_iter, SWIGTYPE_p_tsDeBoorNet.getCPtr(net), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static void ts_bspline_domain(SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_double min, SWIGTYPE_p_double max) {
    tinysplinecsharpPINVOKE.ts_bspline_domain(SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_double.getCPtr(min), SWIGTYPE_p_double.getCPtr(max));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
  }

  public static SWIGTYPE_tsError ts_bspline_is_closed(SWIGTYPE_p_tsBSpline spline, double epsilon, SWIGTYPE_p_int closed, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_is_closed(SWIGTYPE_p_tsBSpline.getCPtr(spline), epsilon, SWIGTYPE_p_int.getCPtr(closed), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_derive(SWIGTYPE_p_tsBSpline spline, uint n, double epsilon, SWIGTYPE_p_tsBSpline derivative, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_derive(SWIGTYPE_p_tsBSpline.getCPtr(spline), n, epsilon, SWIGTYPE_p_tsBSpline.getCPtr(derivative), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_insert_knot(SWIGTYPE_p_tsBSpline spline, double u, uint num, SWIGTYPE_p_tsBSpline result, SWIGTYPE_p_size_t k, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_insert_knot(SWIGTYPE_p_tsBSpline.getCPtr(spline), u, num, SWIGTYPE_p_tsBSpline.getCPtr(result), SWIGTYPE_p_size_t.getCPtr(k), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_split(SWIGTYPE_p_tsBSpline spline, double u, SWIGTYPE_p_tsBSpline split, SWIGTYPE_p_size_t k, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_split(SWIGTYPE_p_tsBSpline.getCPtr(spline), u, SWIGTYPE_p_tsBSpline.getCPtr(split), SWIGTYPE_p_size_t.getCPtr(k), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_tension(SWIGTYPE_p_tsBSpline spline, double tension, SWIGTYPE_p_tsBSpline out_, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_tension(SWIGTYPE_p_tsBSpline.getCPtr(spline), tension, SWIGTYPE_p_tsBSpline.getCPtr(out_), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_to_beziers(SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_tsBSpline beziers, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_to_beziers(SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_tsBSpline.getCPtr(beziers), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_to_json(SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_p_char json, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_to_json(SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_p_char.getCPtr(json), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_from_json(string json, SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_from_json(json, SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_save(SWIGTYPE_p_tsBSpline spline, string path, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_save(SWIGTYPE_p_tsBSpline.getCPtr(spline), path, SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static SWIGTYPE_tsError ts_bspline_load(string path, SWIGTYPE_p_tsBSpline spline, SWIGTYPE_p_tsStatus status) {
    SWIGTYPE_tsError ret = (SWIGTYPE_tsError)tinysplinecsharpPINVOKE.ts_bspline_load(path, SWIGTYPE_p_tsBSpline.getCPtr(spline), SWIGTYPE_p_tsStatus.getCPtr(status));
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static int ts_knots_equal(double x, double y) {
    int ret = tinysplinecsharpPINVOKE.ts_knots_equal(x, y);
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static void ts_arr_fill(SWIGTYPE_p_double arr, uint num, double val) {
    tinysplinecsharpPINVOKE.ts_arr_fill(SWIGTYPE_p_double.getCPtr(arr), num, val);
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
  }

  public static double ts_distance(SWIGTYPE_p_double x, SWIGTYPE_p_double y, uint dimension) {
    double ret = tinysplinecsharpPINVOKE.ts_distance(SWIGTYPE_p_double.getCPtr(x), SWIGTYPE_p_double.getCPtr(y), dimension);
    if (tinysplinecsharpPINVOKE.SWIGPendingException.Pending) throw tinysplinecsharpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static readonly int TS_MAX_NUM_KNOTS = tinysplinecsharpPINVOKE.TS_MAX_NUM_KNOTS_get();
  public static readonly double TS_DOMAIN_DEFAULT_MIN = tinysplinecsharpPINVOKE.TS_DOMAIN_DEFAULT_MIN_get();
  public static readonly double TS_DOMAIN_DEFAULT_MAX = tinysplinecsharpPINVOKE.TS_DOMAIN_DEFAULT_MAX_get();
  public static readonly double TS_KNOT_EPSILON = tinysplinecsharpPINVOKE.TS_KNOT_EPSILON_get();
  public static readonly double TS_CONTROL_POINT_EPSILON = tinysplinecsharpPINVOKE.TS_CONTROL_POINT_EPSILON_get();
}

}
